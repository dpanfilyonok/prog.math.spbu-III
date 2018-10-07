namespace Source
{
    using System;
    using System.Threading;
    using System.Collections.Concurrent;
    
    /// <summary>
    /// My thread pool implementation with fixed thread count
    /// </summary>
    public class MyThreadPool
    {
        /// <summary>
        /// Max avaliable thread count
        /// </summary>
        private readonly int _amountOfThreads;

        /// <summary>
        /// Avaliable threads
        /// </summary>
        private readonly Thread[] _threads;

        /// <summary>
        /// Queue with tasks for execution
        /// </summary>
        private readonly ConcurrentQueue<Action> _tasksQueue;

        /// <summary>
        /// Event, rising when new task was shedulded
        /// </summary>
        private readonly AutoResetEvent _newTaskSheduled;

        /// <summary>
        /// Cancellation token for shutdown thread pool
        /// </summary>
        private readonly CancellationTokenSource _interruptPoolCancellationTokenSource;

        public MyThreadPool(int amountOfThreads)
        {
            _amountOfThreads = amountOfThreads;
            _newTaskSheduled = new AutoResetEvent(false);
            _tasksQueue = new ConcurrentQueue<Action>();
            _interruptPoolCancellationTokenSource = new CancellationTokenSource();
            _threads = new Thread[_amountOfThreads];

            for (int i = 0; i < _amountOfThreads; ++i)
            {
                _threads[i] = new Thread(TryToExecuteTasks) {
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
                else if (_interruptPoolCancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
                else
                {
                    _newTaskSheduled.WaitOne();
                }
            }
        }

        /// <summary>
        /// Shedule task to thread pool (add supplier to the execution queue)
        /// </summary>
        /// <param name="supplier">Task to execute</param>
        /// <typeparam name="TResult">Type of the result of execution</typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Result of execution</returns>
        public IMyTask<TResult> SheduleTask<TResult>(Func<TResult> supplier)
        {
            if (_interruptPoolCancellationTokenSource.IsCancellationRequested)
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

        /// <summary>
        /// Interrupt thread pool: already running tasks are not interrupted, 
        /// but new tasks and tasks from the queue are not accepted for execution by threads from the pool
        /// </summary>
        public void Shutdown() => _interruptPoolCancellationTokenSource.Cancel();
    }
}
