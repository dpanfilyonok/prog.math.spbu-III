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
        private readonly int _maxAmountOfThreads;

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
            _maxAmountOfThreads = amountOfThreads;
            _newTaskSheduled = new AutoResetEvent(false);
            _tasksQueue = new ConcurrentQueue<Action>();
            _interruptPoolCancellationTokenSource = new CancellationTokenSource();
            _threads = new Thread[_maxAmountOfThreads];

            for (int i = 0; i < _maxAmountOfThreads; ++i)
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

        /// <summary>
        /// My task implementation for <see cref="MyThreadPool"/>
        /// </summary>
        /// <typeparam name="TResult">Type of task result</typeparam>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private readonly Func<TResult> _supplier;
            private readonly MyThreadPool _parentThreadPool;
            private readonly ManualResetEvent _executionFinishedEvent;
            private Exception _executionException;

            public MyTask(Func<TResult> task, MyThreadPool parentThreadPool)
            {
                _supplier = task;
                _parentThreadPool = parentThreadPool;
                _executionFinishedEvent = new ManualResetEvent(false);
            }

            public TResult Result
            {
                get
                {   // _executionFinishedEvent.Reset() не нужен тк Result уже вычислен
                    // и есть кто-то ещё захочет получить доступ к результату таска,
                    // он не заблокирует себе навечно поток ожиданием завершения исполнения
                    _executionFinishedEvent.WaitOne();
                    if (_executionException != null)
                    {
                        throw new AggregateException(_executionException);
                    }

                    return Result;
                }

                private set { }
            }
            
            public bool IsCompleted { get; private set; }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
            {
                try
                {
                    var newTask = _parentThreadPool.SheduleTask<TNewResult>(
                        () => supplier(Result)
                    );

                    return newTask;
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            }

            public void ExecuteTaskManually()
            {
                try
                {
                    Result = _supplier.Invoke();
                    IsCompleted = true;
                    _executionFinishedEvent.Set();
                }
                catch (Exception e)
                {
                    _executionException = e;
                }
            }
        }
    }
}
