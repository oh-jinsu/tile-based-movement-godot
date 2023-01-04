using Godot;

namespace Game
{
    using static Constant;

    public class SocketClient : Singleton
    {
        public const string NODE_PATH = "/root/SocketClient";

        private StreamPeerTCP stream;

        public void Connect()
        {
            ThreadPool.Spawn(() =>
            {
                if (stream == null)
                {
                    stream = new StreamPeerTCP();
                }

                stream.ConnectToHost(SOCKET_URI, SOCKET_PORT);
            });

        }

        public bool isConnected
        {
            get
            {
                return stream.GetStatus() == StreamPeerTCP.Status.Connected;
            }
        }

        public void Send(byte[] buffer)
        {

        }

        public void SendTask(byte[] buffer)
        {
            if (stream.GetStatus() != StreamPeerTCP.Status.Connected)
            {
                return;
            }

            ThreadPool.Spawn(() =>
            {
            });
        }
    }
}
