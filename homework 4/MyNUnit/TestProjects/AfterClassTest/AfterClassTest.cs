using System;
using Source.Attributes;

namespace AfterClassTest
{
    public class AfterClassTest
    {
        public static int CheckValue { get; private set; } = 0;

        [AfterClass]
        public static void Increment1() => CheckValue++;

        [AfterClass]
        public static void Increment2() => CheckValue++;
    }
}
