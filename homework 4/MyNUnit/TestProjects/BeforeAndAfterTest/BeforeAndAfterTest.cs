using System;
using Source.Attributes;

namespace BeforeAndAfterTest
{
    public class BeforeAndAfterTest
    {
        public static int CheckValue { get; private set; } = 0;
        private static object lockObject = new object();

        [Before]
        public void IncrementBefore1()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }

        [Before]
        public void IncrementBefore2()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }

        [Test]
        public void VoidTest1() { }

        [Test]
        public void VoidTest2() { }

        [Test]
        public void VoidTest3() { }

        [After]
        public void IncrementAfter1()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }

        [After]
        public void IncrementAfter2()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }
    }
}
