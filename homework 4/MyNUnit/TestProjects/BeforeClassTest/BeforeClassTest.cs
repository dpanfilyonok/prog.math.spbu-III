using System;
using Source.Attributes;

namespace BeforeClassTest
{
    public class BeforeClassTest
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
    }
}
