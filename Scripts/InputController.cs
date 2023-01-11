using Godot;

namespace Game
{
    public class InputController : TouchScreenButton
    {
        private enum Direction
        {
            Idle,
            Up,
            Right,
            Down,
            Left,
        }

        private Network.SocketClient socketClient;

        private MutableObservable<Direction> direction = new()
        {
            Value = Direction.Idle
        };

        public override void _Ready()
        {
            socketClient = Global.Of(this).SocketClient;

            direction.Subscribe(OnDirectionChanged);

            Connect("released", this, nameof(OnReleased));
        }

        private void OnDirectionChanged(Direction direction)
        {
            var b = GetByteFromDirection(direction);

            var packet = new Network.Outgoing.Move
            {
                direction = b
            };

            socketClient.Write(packet);
        }

        private void OnReleased()
        {
            if (direction.Value == Direction.Idle)
            {
                return;
            }

            direction.Value = Direction.Idle;
        }

        public override void _Input(InputEvent @event)
        {
            if (!IsPressed())
            {
                return;
            }

            if (@event is InputEventScreenTouch screenTouch)
            {
                OnScreenTouch(screenTouch.Position);

                return;
            }

            if (@event is InputEventScreenDrag screenDrag)
            {
                OnScreenTouch(screenDrag.Position);

                return;
            }

            if (@event is InputEventMouse mouse)
            {
                OnScreenTouch(mouse.Position);

                return;
            }
        }

        private void OnScreenTouch(Vector2 position)
        {
            var origin = GlobalPosition + new Vector2(64, 64);

            var vector = (position - origin).Normalized();

            var radian = vector.Angle();

            var direction = GetDirection(radian);

            if (direction == this.direction.Value)
            {
                return;
            }

            this.direction.Value = direction;
        }

        private static Direction GetDirection(float radian)
        {
            if (radian > Mathf.Pi * 0.75)
            {
                return Direction.Left;
            }
            else if (radian > Mathf.Pi * 0.25)
            {
                return Direction.Down;
            }
            else if (radian > Mathf.Pi * -0.25)
            {
                return Direction.Right;
            }
            else if (radian > Mathf.Pi * -0.75)
            {
                return Direction.Up;
            }
            else
            {
                return Direction.Left;
            }
        }

        private static byte GetByteFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return 1;
                case Direction.Right: return 2;
                case Direction.Down: return 3;
                case Direction.Left: return 4;
                default: return 0;
            }
        }
    }
}
