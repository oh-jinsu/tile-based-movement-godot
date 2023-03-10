using Godot;
using static Godot.GD;

namespace Game
{
    public class Navigator : Node
    {
        public const string NODE_PATH = "/root/Navigator";

        private MutableObservable<Node> currentScene = new();

        public Observable<Node> CurrentScene
        {
            get
            {
                return currentScene;
            }
        }

        private object arguments;

        public T GetArguments<T>()
        {
            return (T)arguments;
        }

        public override void _Ready()
        {
            var root = GetTree().Root;

            currentScene.Value = root.GetChild(root.GetChildCount() - 1);
        }

        public void GoToSplashScene()
        {
            CallDeferred(nameof(DeferredGoTo), Scene.Splash);
        }

        public void GoToCreationScene()
        {
            CallDeferred(nameof(DeferredGoTo), Scene.Creation);
        }

        public void GoToGameScene(GameSceneArguments arguments)
        {
            this.arguments = arguments;

            CallDeferred(nameof(DeferredGoTo), Scene.Game);
        }

        private void DeferredGoTo(Scene scene)
        {
            currentScene.Value.Free();

            var path = GetPath(scene);

            var nextScene = (PackedScene)Load(path);

            currentScene.Value = nextScene.Instance();

            GetTree().Root.AddChild(currentScene.Value);

            GetTree().CurrentScene = currentScene.Value;
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
