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

            HttpClient.Get(CloudfrontUri("maps/map_0000.yml"), Deserializer.FromYaml<Map, Model.Exception>(OnMapFetched));
        }

        private void OnMapFetched(Map map)
        {
            var mesh = Voxel.CreateMesh(map);

            meshInstance.Mesh = mesh;
        }
    }
}
