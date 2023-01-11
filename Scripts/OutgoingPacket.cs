using System;

namespace Game.Network.Outgoing
{
    public abstract class Packet
    {
        public byte[] Serialize()
        {
            var (serial, payload) = SerializePayload();

            var buffer = new byte[2 + payload.Length];

            Buffer.BlockCopy(serial, 0, buffer, 0, 2);

            Buffer.BlockCopy(payload, 0, buffer, 2, payload.Length);

            return buffer;
        }

        protected abstract (byte[], byte[]) SerializePayload();
    }

    public class Hello : Packet
    {
        public string token;

        protected override (byte[], byte[]) SerializePayload()
        {
            return (new byte[] { 1, 0 }, System.Text.Encoding.ASCII.GetBytes(token));
        }
    }

    public class Move : Packet
    {
        public byte direction;

        protected override (byte[], byte[]) SerializePayload()
        {
            return (new byte[] { 2, 0 }, new byte[] { direction });
        }
    }
}
