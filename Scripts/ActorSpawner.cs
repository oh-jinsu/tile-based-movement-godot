using Godot;

namespace Game
{
    public class ActorSpawner : Node
    {
        [Export]
        private NodePath root;

        [Export]
        private PackedScene actorEntity;

        private Spatial spatial;

        public override void _Ready()
        {
            spatial = GetNode<Spatial>(root);

            var arguments = Global.Of(this).Navigator.GetArguments<GameSceneArguments>();

            var id = arguments.hello.id;

            var actors = arguments.hello.actors;

            foreach (var actor in actors)
            {
                var instance = this.actorEntity.Instance<Actor>();

                instance.Initialize(actor.id, actor.position);

                spatial.CallDeferred("add_child", instance);

                if (id != actor.id)
                {
                    continue;
                }

                var camera = GetNode<FollowingCamera>("../Camera");

                camera.Following = instance;

                var moveController = GetNode<MoveController>("../MoveController");

                moveController.Initialize(instance);
            }
        }
    }
}
