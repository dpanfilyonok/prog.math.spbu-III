using System;

namespace Source
{
    /// <summary>
    /// Provides interface for tasks
    /// </summary>
    /// <typeparam name="TResult">Type of task result</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// Return the result of task.
        /// If task is not completed (<seealso cref="IsCompleted"/>), block running thread and wait for result 
        /// </summary>
        /// <exception cref="AggregateException">Contains information about exception during task</exception>
        /// <value>Evaluated result of the task</value>
        TResult Result { get; }
        
        /// <summary>
        /// Return true, if task is completed
        /// </summary>
        /// <value></value>
        bool IsCompleted { get; }

        /// <summary>
        /// Apply 'supplier' to the result of current task and return new task
        /// </summary>
        /// <param name="supplier"></param>
        /// <typeparam name="TNewResult"></typeparam>
        /// <returns></returns>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier);
    }
}
