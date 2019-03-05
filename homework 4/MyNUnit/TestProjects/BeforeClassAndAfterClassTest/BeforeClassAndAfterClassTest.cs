using System;
using Source.Attributes;

namespace BeforeClassAndAfterClassTest
{
    public class BeforeClassAndAfterClassTest
    {
        public static int CheckValue { get; private set; } = 0;
        private static object lockObject = new object();

        [BeforeClass]
        public static void Increment1()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }

        [BeforeClass]
        public static void Increment2()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }

        [AfterClass]
        public static void Increment3()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }

        [AfterClass]
        public static void Increment4()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }
    }
}
