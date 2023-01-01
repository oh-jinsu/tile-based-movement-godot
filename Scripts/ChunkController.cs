using Godot;

namespace Game
{
    public class ChunkController : BaseNode
    {
        private MeshInstance meshInstance;

        public override void _Ready()
        {
            meshInstance = GetChild<MeshInstance>(0);

            var mesh = Voxel.CreateMesh();

            meshInstance.Mesh = mesh;
        }
    }
}
