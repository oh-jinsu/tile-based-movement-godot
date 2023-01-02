namespace Game
{
    public delegate void StateObserver<T>(T value);

    public interface Observable<T>
    {
        T Value { get; }

        T Subscribe(StateObserver<T> observer);

        void Unsubscribe(StateObserver<T> observer);
    }

    public class MutableObservable<T> : Observable<T>
    {
        private StateObserver<T> observer;

        private T value;

        public T Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;

                observer?.Invoke(this.value);
            }
        }

        public T Subscribe(StateObserver<T> observer)
        {
            this.observer += observer;

            return value;
        }

        public void Unsubscribe(StateObserver<T> observer)
        {
            this.observer -= observer;
        }
    }
}
