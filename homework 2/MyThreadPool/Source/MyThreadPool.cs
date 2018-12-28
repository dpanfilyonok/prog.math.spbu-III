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
        private BlockingCollection<Action<bool>> _tasksQueue;

        /// <summary>
        /// Cancellation token for shutdown thread pool
        /// </summary>
        private readonly CancellationTokenSource _interruptPoolCancellationTokenSource;

        private volatile int _runningThreads;
        private ManualResetEvent _allThreadsFinished;
        private object _lockObject = new object();

        public MyThreadPool(int amountOfThreads)
        {
            _maxAmountOfThreads = amountOfThreads;
            _runningThreads = amountOfThreads;
            _allThreadsFinished = new ManualResetEvent(false);
            _tasksQueue = new BlockingCollection<Action<bool>>();
            _interruptPoolCancellationTokenSource = new CancellationTokenSource();

            _threads = new Thread[_maxAmountOfThreads];
            for (int i = 0; i < _maxAmountOfThreads; ++i)
            {
                var local = i;
                _threads[i] = new Thread(() => TryToExecuteTasks())
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
                if (_interruptPoolCancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    _tasksQueue?.Take(_interruptPoolCancellationTokenSource.Token).Invoke(false);
                }
                catch (OperationCanceledException) { }
            }

            lock (_lockObject)
            {
                _runningThreads--;
            }

            if (_runningThreads == 0)
            {
                _allThreadsFinished.Set();
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
            try
            {
                _tasksQueue.Add(task.ExecuteTaskManually, _interruptPoolCancellationTokenSource.Token);
            }
            catch (OperationCanceledException e)
            {
                throw new InvalidOperationException("", e);
            }
            catch (NullReferenceException e)
            {
                throw new InvalidOperationException("", e);
            }

            return task;
        }

        /// <summary>
        /// Interrupt thread pool: already running tasks are not interrupted, 
        /// but new tasks and tasks from the queue are not accepted for execution by threads from the pool
        /// </summary>
        public void Shutdown()
        {
            _interruptPoolCancellationTokenSource.Cancel();
            _tasksQueue?.CompleteAdding();
            _allThreadsFinished.WaitOne();
            while (!_tasksQueue.IsCompleted)
            {
                _tasksQueue.Take().Invoke(true);
            }

            _tasksQueue = null;
        }

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
            private TResult _result;

            public bool IsCompleted { get; private set; } = false;
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

                    return _result;
                }

                private set => _result = value;
            }

            public MyTask(Func<TResult> task, MyThreadPool parentThreadPool)
            {
                _supplier = task;
                _parentThreadPool = parentThreadPool;
                _executionFinishedEvent = new ManualResetEvent(false);
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
                => _parentThreadPool.SheduleTask<TNewResult>(
                    () => supplier(Result)
                );

            public void ExecuteTaskManually(bool isCancelled = false)
            {
                if (!isCancelled)
                {
                    try
                    {
                        Result = _supplier.Invoke();
                    }
                    catch (Exception e)
                    {
                        _executionException = e;
                    }
                }
                else
                {
                    _executionException = new OperationCanceledException(
                        "Execution cancelled"
                    );
                }

                IsCompleted = true;
                _executionFinishedEvent.Set();
            }
        }
    }
}
