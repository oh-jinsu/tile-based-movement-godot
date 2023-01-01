using Godot;

namespace Game
{
    public class ChunkController : BaseNode
    {
        private MeshInstance meshInstance;

        public override void _Ready()
        {
            meshInstance = GetChild<MeshInstance>(0);

            HttpClient.Get(Constant.GetCloudfrontUri("maps/map_0000.yml"), HttpClient.Deserializer.FromYaml<Map>(OnMapFetched));
        }

        private void OnMapFetched(Map map)
        {
            var mesh = Voxel.CreateMesh(map);

            meshInstance.Mesh = mesh;
        }
    }
}
