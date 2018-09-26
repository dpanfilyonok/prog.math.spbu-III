using Microsoft.VisualStudio.TestTools.UnitTesting;
using Source;
using System;

namespace Tests
{
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSupplierShouldThrowException()
        {
            var lazy = LazyFactory<string>.CreateOneThreadLazy(null);
        }
    }
}