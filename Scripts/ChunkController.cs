using Godot;
using System.Collections.Generic;

namespace Game
{
    using static Constant;

    public class ChunkController : MeshInstance
    {
        private readonly Vector3[] nears = new Vector3[] {
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, -1f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3(0f, -1f, 1f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 1f, 1f),
            new Vector3(0f, -1f, -1f),
            new Vector3(0f, 0f, -1f),
            new Vector3(0f, 1f, -1f),
        };

        private Camera camera;

        private AStar astar = new();

        private Dictionary<Vector3, int> ids = new();

        private MutableObservable<Map> map = new();

        public override void _Ready()
        {
            camera = GetNode<Camera>("../Camera");

            map.Subscribe(OnMapChanged);

            Global.Of(this).ThreadPool.Spawn(FetchMap);
        }

        private async void FetchMap()
        {
            var arguments = Global.Of(this).Navigator.GetArguments<GameSceneArguments>();

            var mapId = arguments.hello.mapId;

            var (ok, response) = await Global.Of(this).HttpClient.GetAsync(CloudfrontUri("maps/" + mapId + ".yml"));

            if (!ok)
            {
                return;
            }

            var model = Deserializer.FromYaml<Map>(response);

            map.Value = model;
        }

        private static int GetUniqueId(Vector3 value)
        {
            return (int)(value.x + 1000 * value.y + 1000000 * value.z);
        }

        private void OnMapChanged(Map map)
        {
            foreach (var position in map.tiles.Keys)
            {
                var id = astar.GetAvailablePointId();

                ids.Add(position, id);

                astar.AddPoint(id, position);
            }

            foreach (var near in map.tiles.Keys)
            {
                var id = ids[near];

                foreach (var direction in nears)
                {
                    var neighborPosition = near + direction;

                    if (!map.tiles.ContainsKey(neighborPosition))
                    {
                        continue;
                    }

                    var neightborId = ids[neighborPosition];

                    if (!astar.HasPoint(neightborId))
                    {
                        continue;
                    }

                    if (astar.ArePointsConnected(id, neightborId))
                    {
                        continue;
                    }

                    astar.ConnectPoints(id, neightborId);
                }
            }

            Mesh = Voxel.CreateMesh(map);

            CreateTrimeshCollision();
        }

        private static Vector3 AdjustTilePosition(Vector3 value)
        {
            return (value + new Vector3(0.5f, 0.01f, 0.5f)).Floor();
        }

        public Vector3[] Getpaths(Vector3 from, Vector3 to)
        {
            var fromTile = AdjustTilePosition(from);

            var toTile = AdjustTilePosition(to);

            if (fromTile == toTile)
            {
                return new Vector3[0];
            }

            if (!ids.ContainsKey(fromTile))
            {
                return new Vector3[0];
            }

            if (!ids.ContainsKey(toTile))
            {
                return new Vector3[0];
            }

            return astar.GetPointPath(ids[fromTile], ids[toTile]);
        }

        public override void _ExitTree()
        {
            map.Unsubscribe(OnMapChanged);
        }
    }
}
