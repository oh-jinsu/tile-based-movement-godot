using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Game
{
    public abstract class HttpResponse { }

    public class HttpClient : Singleton
    {
        public const string NODE_PATH = "/root/HttpClient";

        private const int POLL_INTERVAL = 1;

        public delegate void OnResponse(bool ok, string result);

        private struct RequestParams
        {
            public int serial;

            public Uri uri;

            public object body;

            public OnResponse onResponse;
        }

        private int serial = 0;

        private Dictionary<int, Thread> threads = new Dictionary<int, Thread>();

        private Dictionary<int, RequestParams> requestParams = new Dictionary<int, RequestParams>();

        private Mutex mutex = new Mutex();

        public void Get(Uri uri, OnResponse onResponse = null)
        {
            StartTask(nameof(GetTask), uri, null, onResponse);
        }

        private async void GetTask(int serial)
        {
            try
            {
                var request = PopRequest(serial);

                var client = ConnectClient(request.uri.host);

                string[] headers = { "User-Agent: Pirulo/1.0 (Godot)", "Accept: */*" };

                var (ok, result) = await RequestWith(client, HTTPClient.Method.Get, request.uri.endpoint, headers);

                request.onResponse?.Invoke(ok, result);
            }
            catch (Exception e)
            {
                GD.Print(e);
            }
            finally
            {
                threads.Remove(serial);
            }
        }

        public void Post(Uri uri, object body, OnResponse onResponse = null)
        {
            StartTask(nameof(PostTask), uri, body, onResponse);
        }

        private async void PostTask(int serial)
        {
            try
            {
                var request = PopRequest(serial);

                var client = ConnectClient(request.uri.host);

                string[] headers = { "User-Agent: Pirulo/1.0 (Godot)", "Accept: */*", "Content-Type: application/json;charset=utf-8" };

                var (ok, result) = await RequestWith(client, HTTPClient.Method.Post, request.uri.endpoint, headers, request.body);

                request.onResponse?.Invoke(ok, result);
            }
            catch (Exception e)
            {
                GD.Print(e);
            }
            finally
            {
                threads.Remove(serial);
            }
        }

        private void StartTask(string method, Uri uri, object body, OnResponse onResponse)
        {
            var serial = NewSerial();

            var reservation = new RequestParams
            {
                serial = serial,
                uri = uri,
                body = body,
                onResponse = onResponse,
            };

            mutex.Lock();

            requestParams[serial] = reservation;

            mutex.Unlock();

            var thread = new Thread();

            threads[serial] = thread;

            thread.Start(this, method, serial);
        }

        public int NewSerial()
        {
            return serial++;
        }

        private RequestParams PopRequest(int serial)
        {
            mutex.Lock();

            var request = requestParams[serial];

            var ok = requestParams.Remove(serial);

            if (!ok)
            {
                throw new System.Exception("Request missed");
            }

            mutex.Unlock();

            return request;
        }

        private static HTTPClient ConnectClient(string host)
        {
            var client = new HTTPClient();

            var error = client.ConnectToHost(host);

            if (error != Error.Ok)
            {
                throw new Exception("Failed to connect to host: " + error);
            }

            while (client.GetStatus() == HTTPClient.Status.Connecting || client.GetStatus() == HTTPClient.Status.Resolving)
            {
                OS.DelayMsec(POLL_INTERVAL);

                client.Poll();
            }

            if (client.GetStatus() != HTTPClient.Status.Connected)
            {
                throw new Exception("Failed to connect to host: " + client.GetStatus());
            }

            return client;
        }

        private async Task<(bool, string)> RequestWith(HTTPClient client, HTTPClient.Method method, string endpoint, string[] headers, object body = null)
        {
            var json = body != null ? JsonConvert.SerializeObject(body) : "";

            var error = client.Request(method, endpoint, headers, json);

            if (error != Error.Ok)
            {
                throw new Exception("Failed to request");
            }

            while (client.GetStatus() == HTTPClient.Status.Requesting)
            {
                if (OS.HasFeature("web"))
                {
                    await ToSignal(Engine.GetMainLoop(), "idle_frame");
                }
                else
                {
                    OS.DelayMsec(POLL_INTERVAL);
                }

                client.Poll();
            }

            if (client.GetStatus() != HTTPClient.Status.Body && client.GetStatus() != HTTPClient.Status.Connected)
            {
                return (false, null);
            }

            if (!client.HasResponse())
            {
                return (false, null);
            }

            var buffer = new List<byte>();

            while (client.GetStatus() == HTTPClient.Status.Body)
            {
                var chunk = client.ReadResponseBodyChunk();

                if (chunk.Length == 0)
                {
                    OS.DelayMsec(POLL_INTERVAL);
                }
                else
                {
                    buffer.AddRange(chunk);
                }

                client.Poll();
            }

            var result = Encoding.UTF8.GetString(buffer.ToArray());

            return (client.GetResponseCode().ToString().StartsWith("2"), result);
        }
    }
}
