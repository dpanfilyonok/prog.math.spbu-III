namespace Source
{
    using System;
    using System.Threading;

    /// <summary>
    /// My task implementation for <see cref="MyThreadPool"/>
    /// </summary>
    /// <typeparam name="TResult">Type of task result</typeparam>
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly Func<TResult> _task;
        private readonly MyThreadPool _parentThreadPool;
        private readonly ManualResetEvent _executionFinishedEvent;
        private Exception _executionException;


        public MyTask(Func<TResult> task, MyThreadPool parentThreadPool)
        {
            _task = task;
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

            private set {}
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
                Result = _task.Invoke();
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
