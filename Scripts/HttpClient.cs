using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Newtonsoft.Json;

namespace Game
{
    public class HttpClient : Singleton
    {
        public const string NODE_PATH = "/root/HttpClient";

        private const int POLL_INTERVAL = 1;

        public delegate void OnResponse(string text);

        private struct Request
        {
            public int serial;

            public Uri uri;

            public OnResponse onResponse;
        }

        private int serial = 0;

        private Dictionary<int, Thread> threads = new Dictionary<int, Thread>();

        private Dictionary<int, Request> requests = new Dictionary<int, Request>();

        private Mutex mutex = new Mutex();

        private async void GetTask(int serial)
        {
            mutex.Lock();

            var request = requests[serial];

            var ok = requests.Remove(serial);

            mutex.Unlock();

            if (!ok)
            {
                return;
            }

            var client = new HTTPClient();

            var error = client.ConnectToHost(request.uri.host);

            if (error != Error.Ok)
            {
                throw new Exception("Failed to connect to host");
            }

            while (client.GetStatus() == HTTPClient.Status.Connecting || client.GetStatus() == HTTPClient.Status.Resolving)
            {
                client.Poll();

                OS.DelayMsec(POLL_INTERVAL);
            }

            if (client.GetStatus() != HTTPClient.Status.Connected)
            {
                throw new Exception("Failed to connect to host");
            }

            string[] headers = { "User-Agent: Pirulo/1.0 (Godot)", "Accept: */*" };

            error = client.Request(HTTPClient.Method.Get, request.uri.endpoint, headers);

            if (error != Error.Ok)
            {
                throw new Exception("Failed to request to host");
            }

            while (client.GetStatus() == HTTPClient.Status.Requesting)
            {
                client.Poll();

                if (OS.HasFeature("web"))
                {
                    await ToSignal(Engine.GetMainLoop(), "idle_frame");
                }
                else
                {
                    OS.DelayMsec(POLL_INTERVAL);
                }
            }

            if (client.GetStatus() != HTTPClient.Status.Body && client.GetStatus() != HTTPClient.Status.Connected)
            {
                return;
            }

            if (!client.HasResponse())
            {
                return;
            }


            var buffer = new List<byte>();

            while (client.GetStatus() == HTTPClient.Status.Body)
            {
                client.Poll();

                var chunk = client.ReadResponseBodyChunk();

                if (chunk.Length == 0)
                {
                    OS.DelayMsec(POLL_INTERVAL);
                }
                else
                {
                    buffer.AddRange(chunk);
                }
            }

            var text = Encoding.UTF8.GetString(buffer.ToArray());

            request.onResponse?.Invoke(text);

            threads.Remove(serial);
        }

        public void Get(Uri uri, OnResponse onResponse = null)
        {
            var serial = ++this.serial;

            var request = new Request
            {
                serial = serial,
                uri = uri,
                onResponse = onResponse,
            };

            mutex.Lock();

            requests[serial] = request;

            mutex.Unlock();

            var thread = new Thread();

            threads[serial] = thread;

            thread.Start(this, nameof(GetTask), serial);
        }

        public static class Deserializer
        {
            public delegate void OnResponse<T>(T model);

            private static readonly IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            public static HttpClient.OnResponse FromYaml<T>(OnResponse<T> onResponse)
            {
                return (text) =>
                {
                    onResponse(deserializer.Deserialize<T>(text));
                };
            }

            public static HttpClient.OnResponse FromJson<T>(OnResponse<T> onResponse)
            {
                return (text) =>
                {
                    onResponse(JsonConvert.DeserializeObject<T>(text));
                };
            }
        }
    }
}
