using Game.Network.Incoming;
using Godot;

namespace Game
{
    public class Actor : Spatial
    {
        private string id;

        private Vector3 initialPosition;

        private AnimationPlayer animationPlayer;

        private StateMachine<State> machine;

        public override void _EnterTree()
        {
            GlobalTranslation = initialPosition;
        }

        public override void _Ready()
        {
            Global.Of(this).SocketClient.Subscribe(OnPacketArrived);

            InitializeAnimationPlayer();

            machine = new StateMachine<State>(new State(this));

            machine.Push(new IdleState(this));
        }

        public void Initialize(string id, Vector3 position)
        {
            this.id = id;

            initialPosition = position;
        }

        private void InitializeAnimationPlayer()
        {
            animationPlayer = GetNode<AnimationPlayer>("Model/AnimationPlayer");

            var idle = animationPlayer.GetAnimation("idle");

            idle.Loop = true;

            var run = animationPlayer.GetAnimation("run");

            run.Loop = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            var state = machine.CurrentState;

            if (state == null)
            {
                return;
            }

            state.PhysicsProcess(delta);
        }

        public override void _ExitTree()
        {
            Global.Of(this).SocketClient.Unsubscribe(OnPacketArrived);
        }

        private void OnPacketArrived(Network.Incoming.Packet packet)
        {
            var state = machine.CurrentState;

            if (state == null)
            {
                return;
            }

            state.OnPacketArrived(packet);
        }

        public class State : IState
        {
            protected Actor actor;

            public State(Actor actor)
            {
                this.actor = actor;
            }

            public virtual void OnPacketArrived(Packet packet)
            {
                if (packet is Network.Incoming.Move move)
                {
                    if (move.id != actor.id)
                    {
                        return;
                    }

                    var state = new MoveState(actor, move.destination, move.duration);

                    actor.machine.Shift(state);
                }

                if (packet is Network.Incoming.Stop stop)
                {
                    if (stop.id != actor.id)
                    {
                        return;
                    }

                    var state = new IdleState(actor);

                    actor.machine.Shift(state);
                }
            }

            public virtual void OnPop() { }

            public virtual void OnPush() { }

            public virtual void PhysicsProcess(float delta) { }
        }

        private class IdleState : State
        {
            public IdleState(Actor actor) : base(actor) { }

            public override void OnPush()
            {
                actor.animationPlayer.Play("idle");
            }
        }

        private class MoveState : State
        {
            private Vector3 destination;

            private long duration;

            private Vector3 distance;

            private Vector3 velocity;

            public MoveState(Actor machine, Vector3 destination, long duration) : base(machine)
            {
                this.destination = destination;

                this.duration = duration;

                distance = (destination - actor.GlobalTranslation);

                velocity = 1000 * distance / duration;
            }

            public override void OnPush()
            {
                actor.animationPlayer.Play("run");

                actor.LookAt(destination, Vector3.Up);
            }

            public override void OnPop()
            {
                actor.GlobalTranslation = destination;
            }

            public override void OnPacketArrived(Packet packet)
            {
                if (packet is Network.Incoming.Move move)
                {
                    if (move.id != actor.id)
                    {
                        return;
                    }

                    var state = new MoveState(actor, move.destination, move.duration);

                    actor.machine.Shift(state);
                }

                if (packet is Network.Incoming.Stop stop)
                {
                    if (stop.id != actor.id)
                    {
                        return;
                    }

                    var state = new IdleState(actor);

                    actor.machine.Push(state);
                }
            }

            public override void PhysicsProcess(float delta)
            {
                var deltaVelocity = delta * velocity;

                var distance = destination.DistanceSquaredTo(actor.GlobalTranslation);

                var squaredVelocity = deltaVelocity.LengthSquared();

                if (distance < squaredVelocity)
                {
                    actor.machine.Pop();

                    return;
                }

                actor.GlobalTranslation = actor.GlobalTranslation + deltaVelocity;
            }
        }
    }
}
