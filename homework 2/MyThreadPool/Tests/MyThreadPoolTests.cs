using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Source;

namespace Tests
{
    /// <summary>
    /// Класс, тестирующий работы пула потоков
    /// </summary>
    [TestClass]
    public class MyThreadPoolTests
    {
        private MyThreadPool _pool;
        private const int _threadCount = 2;

        [TestInitialize]
        public void Init()
        {
            _pool = new MyThreadPool(_threadCount);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _pool.Shutdown();
        }

        /// <summary>
        /// Ошибка во время вычисления таска не должно крашить программу
        /// </summary>
        [TestMethod]
        public void ExeptionWhileEvaluatingTaskShouldNotBreakDownMainThread()
        {
            var i = 0;
            _pool.SheduleTask(() => 1 / i);
        }

        /// <summary>
        /// При попытке получить результат выполнения таска, завершившегося с ошибкой должно бросаться исключения
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void TryingToGetResultFromBrokenTaskShouldRaiseException()
        {
            var i = 0;
            var task = _pool.SheduleTask(() => 1 / i);
            var result = task.Result;
        }

        /// <summary>
        /// Ошибка во время вычисления продолжения таска не должно крашить программу
        /// </summary>
        [TestMethod]
        public void ExeptionInContinueWithShouldNotBreakDownMainThread()
        {
            var task = _pool.SheduleTask(() => 1);
            task.ContinueWith(i => 1 / (i - i));
        }

        /// <summary>
        /// При попытке получить результат выполнения продолжения таска, завершившегося с ошибкой должно бросаться исключения
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void TryingToGetResultFromBrokenContinueWithShouldRaiseException()
        {
            var task = _pool.SheduleTask(() => 1);
            var newTask = task.ContinueWith(i => 1 / (i - i));
            var result = newTask.Result;
        }

        /// <summary>
        /// Задачи, находящиейся в пуле потоков, должны завершить свое выполнение и после завершения работы пула
        /// </summary>
        [TestMethod]
        public void ShutdownedPoolShouldContinueExecuteTasksInIt()
        {
            var a = 1;
            var task = _pool.SheduleTask(() =>
            {
                Thread.Sleep(100);
                return a;
            });
            var newTask = task.ContinueWith(i =>
            {
                Thread.Sleep(100);
                return i + i;
            });

            _pool.Shutdown();

            Assert.AreEqual(a, task.Result);
            Assert.AreEqual(a + a, newTask.Result);
        }

        /// <summary>
        /// При попытке добавить задачу в пул потоков после его завершения должно кидаться исключения
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddingTasksToPoolAfterShutdowningShouldRaiseProperException()
        {
            var task = _pool.SheduleTask(() =>
            {
                Thread.Sleep(100);
                return 1;
            });

            _pool.Shutdown();

            _pool.SheduleTask(() => 2);
        }

        /// <summary>
        /// При попытке добавить продолжение задачи в пул потоков после его завершения должно кидаться исключения
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UsingContinueWithAfterPoolShutdowningShouldRaiseProperException()
        {
            var task = _pool.SheduleTask(() =>
            {
                Thread.Sleep(100);
                return 1;
            });

            _pool.Shutdown();

            task.ContinueWith(i => i * i);
        }

        /// <summary>
        /// Цепочка продолжений 1 задачи должна исполняться корректно
        /// </summary>
        [TestMethod]
        public void SequenceOfContinueWithShouldWorkCorrectly()
        {
            var task = _pool.SheduleTask(() =>
            {
                Thread.Sleep(100);
                return 2;
            });

            task.ContinueWith(i => i * 1)
                .ContinueWith(i => i * 10)
                .ContinueWith(i => i * 100)
                .ContinueWith(i => i * 1000);

            Assert.AreEqual(2000000, task.Result);
        }

        /// <summary>
        /// Если во время выполнения цепочки продолжения задачи возникло исключение, 
        /// то при доступе к ее результату должно бросаться исключение
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExceptionInSequenceOfContinueWithShouldRaiseProperException()
        {
            var task = _pool.SheduleTask(() =>
            {
                Thread.Sleep(100);
                return 2;
            });

            var result = task.ContinueWith(i => i * 1)
                .ContinueWith(i => i * 10)
                .ContinueWith(i => i / (i - i))
                .ContinueWith(i => i * 1000).Result;
        }

        /// <summary>
        /// Если во время выполнения цепочки продолжения задачи возникло исключение, 
        /// то рассмотрим поведение :)
        /// </summary>
        [TestMethod]
        public void ExceptionInSequenceOfContinueBehaviorCheck()
        {
            var a = 0;
            try
            {
                var result = _pool.SheduleTask(() =>
                {
                    a++;
                    return 0;
                }).ContinueWith(i =>
                {
                    a++;
                    return 0;
                }).ContinueWith(i =>
                {
                    a++;
                    return 0;
                })
                .ContinueWith(i =>
                {
                    a++;
                    return 1 / i;
                })
                .ContinueWith(i =>
                {
                    a++;
                    return 0;
                }).Result;
            }
            catch (Exception) { }

            Assert.AreEqual(3, a);
        }

        /// <summary>
        /// Тест на наличие гонок
        /// </summary>
        [TestMethod]
        public void RaceConditionTest()
        {
            var a = 0;
            const int iterCount = 10;
            Func<int> supplier = () =>
            {
                for (int i = 0; i < iterCount; ++i)
                {
                    a++;
                }

                return 0;
            };

            const int amountOfTasks = 100;
            var tasks = new IMyTask<int>[amountOfTasks];
            for (int i = 0; i < amountOfTasks; ++i)
            {
                tasks[i] = _pool.SheduleTask(supplier);
            }

            foreach (var task in tasks)
            {
                Assert.AreEqual(0, task.Result);
            }

            Assert.AreEqual(amountOfTasks * iterCount, a);
        }
    }
}
