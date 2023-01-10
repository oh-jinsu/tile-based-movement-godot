using Godot;

namespace Game
{
    using static Constant;

    public class ChunkController : MeshInstance
    {
        public override void _Ready()
        {
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

            OnMapFetched(model);
        }

        private void OnMapFetched(Map map)
        {
            var mesh = Voxel.CreateMesh(map);

            Mesh = mesh;
        }
    }
}
