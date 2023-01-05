using System;

namespace Game.Packet
{
    public abstract class Outgoing
    {
        public abstract int Serial();

        public byte[] Serialize()
        {
            var serial = BitConverter.GetBytes(Convert.ToUInt16(Serial()));

            var payload = SerializePayload();

            var buffer = new byte[2 + payload.Length];

            Buffer.BlockCopy(serial, 0, buffer, 0, 2);

            Buffer.BlockCopy(payload, 0, buffer, 2, payload.Length);

            return buffer;
        }

        protected abstract byte[] SerializePayload();
    }

    public class Hello : Outgoing
    {
        public string token;

        public override int Serial() => 1;

        protected override byte[] SerializePayload()
        {
            return System.Text.Encoding.ASCII.GetBytes(token);
        }
    }
}
