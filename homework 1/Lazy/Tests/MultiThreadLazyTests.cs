using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Source;

namespace Tests
{
    [TestClass]
    public class MultiThreadLazyTests
    {
        const int _threadsCount = 1000;
        Thread[] _threads = new Thread[_threadsCount];

        [TestMethod]
        public void MultiThreadGetShouldReturnTheSameObject()
        {
            string testString = "hello";
            Func<string> supplier = () => testString;

            var lazy = LazyFactory<string>.CreateMultiThreadLazy(supplier);
            for (int i = 0; i < _threadsCount; ++i)
            {
                _threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 10; ++j)
                    {
                        Assert.AreSame(testString, lazy.Get());
                    }
                });
            }

            foreach (var thread in _threads)
            {
                thread.Start();
            }

            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }

        [TestMethod]
        public void MultiTreadSupplierShouldBeCalculatedOnce()
        {
            int counter = 0;
            var supplier = new Func<int>(() =>
            {
                counter++;
                return counter;
            });

            var lazy = LazyFactory<int>.CreateOneThreadLazy(supplier);
            for (int i = 0; i < _threadsCount; ++i)
            {
                _threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 10; ++j)
                    {
                        Assert.AreEqual(1, lazy.Get());
                    }
                });
            }

            foreach (var thread in _threads)
            {
                thread.Start();
            }

            Thread.Sleep(200);

            foreach (var thread in _threads)
            {
                thread.Join();
            }

            Assert.AreEqual(1, counter);
        }
    }
}