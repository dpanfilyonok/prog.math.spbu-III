using System;
using Source.Attributes;

namespace AfterClassTest
{
    public class AfterClassTest
    {
        public static int CheckValue { get; private set; } = 0;
        private static object lockObject = new object();

        [AfterClass]
        public static void Increment1()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }

        [AfterClass]
        public static void Increment2()
        {
            lock (lockObject)
            {
                CheckValue++;
            }
        }
    }
}
