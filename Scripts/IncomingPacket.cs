using Godot;
using System;
using System.Collections.Generic;

namespace Game.Network.Incoming
{
    public abstract class Packet
    {
        public static Packet Deserialize(byte[] buffer)
        {
            var serial = BitConverter.ToUInt16(buffer, 0);

            var bytes = new byte[buffer.Length - 2];

            Buffer.BlockCopy(buffer, 2, bytes, 0, bytes.Length);

            switch (serial)
            {
                case 1:
                    return Hello.Deserialize(bytes);
                case 2:
                    return Move.Deserialize(bytes);
                case 3:
                    return Stop.Deserialize(bytes);
                default:
                    throw new Exception("Unexpected packet");
            }
        }
    }

    public class Hello : Packet
    {
        public class Actor
        {
            public string id;

            public Vector3 position;
        }

        public string id;

        public string mapId;

        public List<Actor> actors;

        public static new Hello Deserialize(byte[] bytes)
        {
            var id = System.Text.Encoding.UTF8.GetString(bytes, 0, 32);

            var mapId = System.Text.Encoding.UTF8.GetString(bytes, 32, 8);

            var users = new List<Actor>();

            for (int i = 40; i < bytes.Length; i += 44)
            {
                var actorId = System.Text.Encoding.UTF8.GetString(bytes, i, 32);

                var x = BitConverter.ToInt32(bytes, i + 32);

                var y = BitConverter.ToInt32(bytes, i + 36);

                var z = BitConverter.ToInt32(bytes, i + 40);

                var user = new Actor
                {
                    id = actorId,
                    position = new Vector3(x, y, z),
                };

                users.Add(user);
            }

            return new Hello
            {
                id = id,
                mapId = mapId,
                actors = users,
            };
        }
    }

    public class Move : Packet
    {
        public string id;

        public Vector3 destination;

        public long duration;

        public static new Move Deserialize(byte[] bytes)
        {
            var id = System.Text.Encoding.UTF8.GetString(bytes, 0, 32);

            var x = BitConverter.ToInt32(bytes, 32);

            var y = BitConverter.ToInt32(bytes, 36);

            var z = BitConverter.ToInt32(bytes, 40);

            var duration = BitConverter.ToInt64(bytes, 44);

            return new Move
            {
                id = id,
                destination = new Vector3(x, y, z),
                duration = duration,
            };
        }
    }

    public class Stop : Packet
    {
        public string id;

        public Vector3 position;

        public static new Stop Deserialize(byte[] bytes)
        {
            var id = System.Text.Encoding.UTF8.GetString(bytes, 0, 32);

            var x = BitConverter.ToInt32(bytes, 32);

            var y = BitConverter.ToInt32(bytes, 36);

            var z = BitConverter.ToInt32(bytes, 40);

            return new Stop
            {
                id = id,
                position = new Vector3(x, y, z),
            };
        }
    }
}
