using Godot;

namespace Game {
    public class InputController : Node
    {
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton inputEvent) {
                var packet = new Network.Outgoing.Move { 
                    direction = 1
                };

                Global.Of(this).SocketClient.Write(packet.Serialize());
            }
        }
    }
}