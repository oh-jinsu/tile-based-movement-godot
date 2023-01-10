using Godot;
using System;

namespace Game.Network
{
    using static Constant;

    public delegate void PacketObserver(Network.Incoming.Packet packet);

    public class SocketClient : Node
    {
        private PacketObserver observer;

        public const string NODE_PATH = "/root/SocketClient";

        private const int TICK = 500;

        private StreamPeerTCP stream;

        public bool Connect(int timeout = 5000)
        {
            stream ??= new StreamPeerTCP();

            stream.ConnectToHost(SOCKET_URI, SOCKET_PORT);

            while(timeout > 0) {
                if (stream.GetStatus() == StreamPeerTCP.Status.Connected) {
                    GD.Print("socket connected");

                    return true;
                }

                timeout -= TICK;

                OS.DelayMsec(TICK);
            }

            return false;
        }

        public override void _Process(float delta)
        {
            if (stream == null) {
                return;
            }

            if (stream.GetStatus() != StreamPeerTCP.Status.Connected)
            {
                return;
            }

            var length = stream.GetAvailableBytes();

            if (length == 0) {
                return;
            }

            var result = stream.GetPartialData(length);

            if ((Error)result[0] != Error.Ok) {
                Disconnect();

                return;
            }

            var bytes = (byte[])result[1];

            var size = BitConverter.ToUInt16(bytes, 0);

            if (size != bytes.Length - 2) {
                GD.Print("bytes not enough");

                return;
            } 

            var buffer = new byte[size];

            Buffer.BlockCopy(bytes, 2, buffer, 0, buffer.Length);

            var packet = Network.Incoming.Packet.Deserialize(buffer);
        
            observer?.Invoke(packet);
        }

        public void Write(byte[] data)
        {
            if (stream == null) {
                return;
            }

            if (stream.GetStatus() != StreamPeerTCP.Status.Connected)
            {
                return;
            }

            var buffer = new byte[2 + data.Length];

            var size = BitConverter.GetBytes(System.Convert.ToUInt16(data.Length));

            Buffer.BlockCopy(size, 0, buffer, 0, 2);

            Buffer.BlockCopy(data, 0, buffer, 2, data.Length);

            if (stream.PutData(buffer) != Error.Ok) {
                Disconnect();
            }
        }

        public void Subscribe(PacketObserver observer) {
            this.observer += observer;
        }

        public void Unsubscribe(PacketObserver observer) {
            this.observer -= observer;
        }

        public void Disconnect() {
            if (stream == null) {
                return;
            }

            stream.DisconnectFromHost();

            stream = null;

            GD.Print("socket disconnected");
        }

        public override void _ExitTree()
        {
            Disconnect();
        }
    }
}
