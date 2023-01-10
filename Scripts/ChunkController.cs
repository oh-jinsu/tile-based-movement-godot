using Godot;

namespace Game
{
    using static Constant;

    public class ChunkController : BaseNode
    {
        private MeshInstance meshInstance;

        public override void _Ready()
        {
            meshInstance = GetChild<MeshInstance>(0);

            ThreadPool.Spawn(FetchMap);
        }

        private async void FetchMap()
        {
            var arguments = Navigator.GetArguments<GameSceneArguments>();

            var mapId = arguments.hello.mapId;

            var (ok, response) = await HttpClient.GetAsync(CloudfrontUri("maps/" + mapId + ".yml"));

            if (!ok)
            {
                return;
            }

            var model = Deserializer.FromYaml<Map>(response);

            OnMapFetched(model);
        }

        private void OnMapFetched(Map map)
        {
            var mesh = Voxel.CreateMesh(map);

            meshInstance.Mesh = mesh;
        }
    }
}
