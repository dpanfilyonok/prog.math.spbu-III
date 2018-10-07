namespace Source
{
    using System;
    using System.Threading;

    public class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly Func<TResult> _task;
        private Exception _executionException;
        private ManualResetEvent _executionFinishedEvent;

        public MyTask(Func<TResult> task)
        {
            _task = task;
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
        
        public IMyTask<TNewResult> ContinueWith<TNewResult>(System.Func<TResult, TNewResult> supplier)
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteTaskManually()
        {
            try
            {
                Result = _task();
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
