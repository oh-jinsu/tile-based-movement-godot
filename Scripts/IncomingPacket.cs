using Godot;
using System;
using System.Collections.Generic;

namespace Game.Network.Incoming
{
    public abstract class Packet
    {
        public abstract int Serial();

        public static Packet Deserialize(byte[] buffer) {
            var serial = BitConverter.ToUInt16(buffer, 0);

            var bytes = new byte[buffer.Length - 2];

            Buffer.BlockCopy(buffer, 2, bytes, 0, bytes.Length);

            switch (serial) {
                case 1:
                    return Hello.Deserialize(bytes);
                default:
                    throw new Exception("Unexpected packet");
            }
        }
    }

    public class Hello : Packet
    {
        public class User {
            public string id;

            public Vector3 position;
        }

        public override int Serial() => 1;

        public string mapId;

        public List<User> users;

        public static new Hello Deserialize(byte[] bytes) {
            var mapIdBuffer = new byte[8];

            Buffer.BlockCopy(bytes, 0, mapIdBuffer, 0, 8);

            var mapId = System.Text.Encoding.UTF8.GetString(mapIdBuffer);

            var users = new List<User>();

            for (int i = 8; i < bytes.Length; i += 44) {
                var id = System.Text.Encoding.UTF8.GetString(bytes, i, 32);

                var x = BitConverter.ToInt32(bytes, i + 32);

                var y = BitConverter.ToInt32(bytes, i + 36);

                var z = BitConverter.ToInt32(bytes, i + 40);

                var user = new User {
                    id = id, 
                    position = new Vector3(x, y, z),    
                };

                users.Add(user);
            }

            return new Hello {
                mapId = mapId,
                users = users,
            };
        }
    }
}
