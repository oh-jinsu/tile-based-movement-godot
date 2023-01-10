using Godot;

namespace Game {
    public class PlayerSpawner : Node {
        [Export]
        private NodePath root;

        [Export]
        private PackedScene player;

        private Spatial spatial;

        public override void _Ready()
        {
            spatial = GetNode<Spatial>(root);

            var instance = player.Instance<Player>();

            spatial.CallDeferred("add_child", instance);

            var camera = GetNode<FollowingCamera>("../Camera");

            camera.Following = instance;

        }
    }
}