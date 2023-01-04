using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Game
{
    public delegate Task ThreadPoolTask();

    public class ThreadPool : BaseNode
    {
        public const string NODE_PATH = "/root/ThreadPool";

        private int serial;

        private Mutex mutex = new();

        private Dictionary<int, Thread> threads = new();

        private Dictionary<int, ThreadPoolTask> tasks = new();

        public void Spawn(ThreadPoolTask task)
        {
            var i = serial++;

            var thread = new Thread();

            mutex.Lock();

            tasks[i] = task;

            threads[i] = thread;

            mutex.Unlock();

            thread.Start(this, nameof(Execute), i);
        }

        public void Spawn(Action task)
        {
            Spawn(() => Task.Run(task));
        }

        private async void Execute(int i)
        {
            try
            {
                mutex.Lock();

                var task = tasks[i];

                mutex.Unlock();

                if (task == null)
                {
                    throw new System.Exception("missing argument");
                }

                await task();
            }
            catch (Exception e)
            {
                GD.Print(e);
            }
            finally
            {
                mutex.Lock();

                tasks.Remove(i);

                threads.Remove(i);

                mutex.Unlock();
            }
        }
    }
}
