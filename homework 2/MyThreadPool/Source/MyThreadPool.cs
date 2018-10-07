using System.Threading;
using System.Collections.Concurrent;
using System;

namespace Source
{
    public class MyThreadPool
    {
        private readonly int _amountOfThreads;
        private readonly Thread[] _threads;
        private readonly ConcurrentQueue<Action> _tasksQueue;
        private readonly AutoResetEvent _newTaskSheduled;
        private readonly CancellationTokenSource _interruptPoolTokenSource;

        public MyThreadPool(int amountOfThreads)
        {
            _amountOfThreads = amountOfThreads;
            _newTaskSheduled = new AutoResetEvent(false);
            _tasksQueue = new ConcurrentQueue<Action>();
            _interruptPoolTokenSource = new CancellationTokenSource();
            _threads = new Thread[_amountOfThreads];

            for (int i = 0; i < _amountOfThreads; ++i)
            {
                _threads[i] = new Thread(TryToExecuteTasks)
                {
                    IsBackground = true,
                    Name = $"Thread {i}"
                };

                _threads[i].Start();
            }
        }

        private void TryToExecuteTasks()
        {
            while (true)
            {
                if (_tasksQueue.TryDequeue(out Action task))
                {
                    task.Invoke();
                }
                else if (_interruptPoolTokenSource.IsCancellationRequested)
                {
                    break;
                }
                else
                {
                    _newTaskSheduled.WaitOne();
                }
            }
        }

        public IMyTask<TResult> SheduleTask<TResult>(Func<TResult> supplier)
        {
            if (_interruptPoolTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException(
                    "Current threadpool was shutdown, so you cant shedule tasks anymore"
                );
            }

            var task = new MyTask<TResult>(supplier, this);
            _tasksQueue.Enqueue(task.ExecuteTaskManually);
            _newTaskSheduled.Set();

            return task;
        }

        public void Shutdown() => _interruptPoolTokenSource.Cancel();
    }
}
