using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Game.Network
{
    public class HttpClient : Node
    {
        public const string NODE_PATH = "/root/HttpClient";

        private const int POLL_INTERVAL = 1;

        public delegate void OnResponse(bool ok, string result);

        public async Task<(bool, string)> GetAsync(Uri uri)
        {
            var client = ConnectClient(uri.host);

            string[] headers = { "User-Agent: Pirulo/1.0 (Godot)", "Accept: */*" };

            return await RequestAsyc(client, HTTPClient.Method.Get, uri.endpoint, headers);
        }

        public async Task<(bool, string)> PostAsync(Uri uri, object body)
        {
            var client = ConnectClient(uri.host);

            string[] headers = { "User-Agent: Pirulo/1.0 (Godot)", "Accept: */*", "Content-Type: application/json;charset=utf-8" };

            return await RequestAsyc(client, HTTPClient.Method.Post, uri.endpoint, headers, body);
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

        private async Task<(bool, string)> RequestAsyc(HTTPClient client, HTTPClient.Method method, string endpoint, string[] headers, object body = null)
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
