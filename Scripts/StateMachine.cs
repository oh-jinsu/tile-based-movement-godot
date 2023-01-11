using System.Collections.Generic;

namespace Game
{
    public interface IState
    {
        void OnPush();

        void OnPop();
    }

    public class StateMachine<T> where T : IState
    {
        private Queue<T> queue = new();

        private T currentState;

        public T DefaultState;

        public StateMachine(T state)
        {
            DefaultState = state;
        }

        public StateMachine() : this(default(T)) { }

        public T CurrentState
        {
            get
            {
                if (currentState != null)
                {
                    return currentState;
                }

                if (queue.Count > 0)
                {
                    currentState = queue.Dequeue();

                    currentState.OnPush();
                }
                else
                {
                    currentState = DefaultState;
                }

                return currentState;
            }
        }

        public void Push(T state)
        {
            queue.Enqueue(state);
        }

        public void Pop()
        {
            currentState.OnPop();

            currentState = default(T);
        }

        public void Shift(T state)
        {
            Push(state);

            Pop();
        }
    }
}
