using Godot;
using System.Collections.Generic;

namespace Game
{
    public struct Map
    {
        public string id;

        public string version;

        public string name;

        public Dictionary<Vector3, Tile> tiles;

        public Dictionary<Vector3, StaticObject> objects;
    }

    public struct Tile
    {
        public string id;

        public byte rotation;
    }

    public struct StaticObject
    {

    }
}
