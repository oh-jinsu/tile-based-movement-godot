using Godot;
using System.Collections.Generic;

namespace Game
{
    public class MoveController : Node
    {
        private Camera camera;

        private ChunkController chunkController;

        private Actor actor;

        private Network.SocketClient socketClient;

        private Queue<Vector3> pathQueue = new();

        private Vector3 currentPosition;

        public override void _Ready()
        {
            camera = GetNode<Camera>("../Camera");

            chunkController = GetNode<ChunkController>("../ChunkController");

            socketClient = Global.Of(this).SocketClient;

            socketClient.Subscribe(OnPacketArrived);
        }

        public void Initialize(Actor actor)
        {
            this.actor = actor;

            this.currentPosition = Vector3.Zero;
        }

        private void RequestMove(byte direction)
        {
            var packet = new Network.Outgoing.Move
            {
                direction = direction
            };

            socketClient.Write(packet);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsPressed())
            {
                return;
            }

            // if (@event is InputEventScreenTouch screenTouch)
            // {
            //     MoveTo(screenTouch.Position);

            //     return;
            // }

            if (@event is InputEventMouseButton mouse)
            {
                MoveTo(mouse.Position);

                return;
            }
        }

        public void MoveTo(Vector2 screenPosition)
        {
            if (actor == null)
            {
                return;
            }

            var space = chunkController.GetWorld().DirectSpaceState;

            var from = camera.ProjectRayOrigin(screenPosition);

            var to = camera.ProjectPosition(screenPosition, 1000);

            var intersection = space.IntersectRay(from, to);

            if (intersection.Count == 0)
            {
                return;
            }

            var point = ((Vector3)intersection["position"]);

            var position = currentPosition;

            var paths = chunkController.Getpaths(position, point);

            if (paths.Length < 2)
            {
                return;
            }

            pathQueue.Clear();

            for (int i = 1; i < paths.Length; i++)
            {
                if (i == 1)
                {
                    var vector = position.DirectionTo(paths[i]);

                    var direction = GetByteFromVector(vector);

                    RequestMove(direction);

                    continue;
                }

                pathQueue.Enqueue(paths[i]);
            }
        }

        private void OnPacketArrived(Network.Incoming.Packet packet)
        {
            if (packet is Network.Incoming.Move move)
            {
                currentPosition = move.destination;

                if (pathQueue.Count == 0)
                {
                    RequestMove(0);

                    return;
                }

                var path = pathQueue.Dequeue();

                var vector = move.destination.DirectionTo(path);

                var direction = GetByteFromVector(vector);

                RequestMove(direction);
            }
        }

        private static byte GetByteFromVector(Vector3 vector)
        {
            if (vector.z > 0)
            {
                return 1;
            }

            if (vector.z < 0)
            {
                return 3;
            }

            if (vector.x < 0)
            {
                return 2;
            }

            if (vector.x > 0)
            {
                return 4;
            }

            return 0;
        }

        public override void _ExitTree()
        {
            socketClient.Unsubscribe(OnPacketArrived);
        }
    }
}
