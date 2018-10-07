using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System;

namespace Source
{
    public class MyThreadPool
    {
        private readonly int _amountOfThreads;

        private Thread[] _threads;

        private ConcurrentQueue<Action> _tasksQueue;

        private AutoResetEvent _newTaskSheduledEvent;

        private CancellationTokenSource _interruptPoolTokenSource;

        public MyThreadPool(int amountOfThreads)
        {
            _amountOfThreads = amountOfThreads;
            _newTaskSheduledEvent = new AutoResetEvent(false);
            _tasksQueue = new ConcurrentQueue<Action>();
            _interruptPoolTokenSource = new CancellationTokenSource();
            _threads = new Thread[_amountOfThreads];

            for (int i = 0; i < _amountOfThreads; ++i)
            {
                _threads[i] = new Thread(TryToExecuteTask)
                {
                    IsBackground = true,
                    Name = $"Thread {i}"
                };

                _threads[i].Start();
            }
        }

        private void TryToExecuteTask()
        {
            while (true)
            {
                if (_tasksQueue.TryDequeue(out Action task))
                {
                    task.Invoke();
                }
                else
                {
                    _newTaskSheduledEvent.WaitOne();
                }
            }
        }

        public IMyTask<TResult> SheduleTask<TResult>(Func<TResult> supplier)
        {
            var task = new MyTask<TResult>(supplier);
            _tasksQueue.Enqueue(task.ExecuteTaskManually);
            _newTaskSheduledEvent.Set();

            return task;
        }

        public void Shutdown()
        {
            _interruptPoolTokenSource.Cancel();
        }
    }
}
