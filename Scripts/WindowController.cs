using Godot;

namespace Game
{
    public class WindowController : Node
    {
        public const string NODE_PATH = "/root/WindowController";

        private AcceptDialog dialog;

        public override void _Ready()
        {
            var node = Global.Of(this).Navigator.CurrentScene.Subscribe(OnCurrentSceneChange);

            OnCurrentSceneChange(node);
        }

        private void OnCurrentSceneChange(Node node)
        {
            var scene = GD.Load<PackedScene>("res://Scenes/DialogScene.tscn");

            dialog = scene.Instance<AcceptDialog>();

            node.AddChild(dialog);
        }

        public void PopupDialog(string message)
        {
            dialog.DialogText = message;

            dialog.PopupCentered(new Vector2(288, 160));
        }

        public override void _ExitTree()
        {
            Global.Of(this).Navigator.CurrentScene.Unsubscribe(OnCurrentSceneChange);
        }
    }
}
