using Godot;
using System;

namespace Game
{
    using static Constant;

    public class SocketClient : Singleton
    {
        public const string NODE_PATH = "/root/SocketClient";

        private StreamPeerTCP stream;

        public bool Connect()
        {
            if (stream == null)
            {
                stream = new StreamPeerTCP();
            }

            stream.ConnectToHost(SOCKET_URI, SOCKET_PORT);

            return stream.IsConnectedToHost();
        }

        public bool IsConnectedToHost
        {
            get
            {
                return stream?.IsConnectedToHost() == true;
            }
        }

        public void Write(byte[] data)
        {
            if (stream?.GetStatus() != StreamPeerTCP.Status.Connected)
            {
                return;
            }

            var buffer = new byte[2 + data.Length];

            var size = BitConverter.GetBytes(System.Convert.ToUInt16(data.Length));

            Buffer.BlockCopy(size, 0, buffer, 0, 2);

            Buffer.BlockCopy(data, 0, buffer, 2, data.Length);

            stream.PutData(buffer);
        }
    }
}
