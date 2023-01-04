using Godot;

namespace Game
{
    using static GD;
    using static Constant;

    public class SocketClient : Singleton
    {
        public const string NODE_PATH = "/root/SocketClient";

        private StreamPeerTCP stream;

        public override void _Ready()
        {
            if (stream == null)
            {
                stream = new StreamPeerTCP();
            }

            stream.ConnectToHost(SOCKET_URI, SOCKET_PORT);

            if (stream.IsConnectedToHost())
            {
                Print("socket connected");
            }
        }

        public void Send(byte[] buffer)
        {
            if (stream?.GetStatus() != StreamPeerTCP.Status.Connected)
            {
                return;
            }
        }
    }
}
