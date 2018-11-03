using Microsoft.VisualStudio.TestTools.UnitTesting;
using Source;
using System;

namespace Tests
{
    /// <summary>
    /// Тесты на корректность работы <see cref="OneThreadLazy">
    /// </summary>
    [TestClass]
    public class OneThreadLazyTests
    {
        [TestMethod]
        public void GetShouldReturnTheSameObject()
        {
            string testString = "hello";
            Func<string> supplier = () => testString;

            var lazy = LazyFactory<string>.CreateOneThreadLazy(supplier);
            var obj1 = lazy.Get();
            var obj2 = lazy.Get();

            Assert.AreSame(obj1, obj2);
        }

        [TestMethod]
        public void SupplierShouldBeCalculatedOnce()
        {
            int counter = 0;
            var supplier = new Func<int>(() =>
            {
                counter++;
                return counter;
            });

            var lazy = LazyFactory<int>.CreateOneThreadLazy(supplier);
            var obj1 = lazy.Get();
            var obj2 = lazy.Get();

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSupplierShouldThrowException()
        {
            var lazy = LazyFactory<string>.CreateOneThreadLazy(null);
        }

        [TestMethod]
        public void SupplierReturningNullShouldWorkCorrectly()
        {
            var lazy = LazyFactory<int?>.CreateOneThreadLazy(() => null);
            var obj1 = lazy.Get();
            var obj2 = lazy.Get();

            Assert.IsNull(obj1);
            Assert.IsNull(obj2);
        }
    }
}