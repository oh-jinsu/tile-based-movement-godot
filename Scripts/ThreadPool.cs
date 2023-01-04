using Godot;
using System;
using System.Collections.Generic;

namespace Game
{
    public class ThreadPool : BaseNode
    {
        public const string NODE_PATH = "/root/ThreadPool";

        private int serial;

        private Mutex mutex = new();

        private Dictionary<int, Thread> threads = new();

        private Dictionary<int, Action> tasks = new();

        public void Spawn(Action task)
        {
            var i = serial++;

            var thread = new Thread();

            mutex.Lock();

            tasks[i] = task;

            threads[i] = thread;

            mutex.Unlock();

            thread.Start(this, nameof(Execute), i);
        }

        private void Execute(int i)
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

                task();
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
