using Godot;
using static Godot.GD;

namespace Game
{
    public class Navigator : Singleton
    {
        public const string NODE_PATH = "/root/Navigator";

        public Node CurrentScene { get; private set; }

        public override void _Ready()
        {
            var root = GetTree().Root;

            CurrentScene = root.GetChild(root.GetChildCount() - 1);
        }

        public void GoTo(Scene scene)
        {
            CallDeferred(nameof(DeferredGoTo), scene);
        }

        private void DeferredGoTo(Scene scene)
        {
            CurrentScene.Free();

            var path = GetPath(scene);

            var nextScene = (PackedScene)Load(path);

            CurrentScene = nextScene.Instance();

            GetTree().Root.AddChild(CurrentScene);

            GetTree().CurrentScene = CurrentScene;
        }

        private static string GetPath(Scene scene)
        {
            switch (scene)
            {
                case Scene.Splash:
                    return "res://Scenes/SplashScene.tscn";
                case Scene.Creation:
                    return "res://Scenes/CreationScene.tscn";
                case Scene.Game:
                    return "res://Scenes/GameScene.tscn";
                default:
                    throw new System.Exception("Could not get the path");
            }
        }
    }
}
