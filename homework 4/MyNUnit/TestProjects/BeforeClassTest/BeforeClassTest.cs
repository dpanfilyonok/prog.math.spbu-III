using System;
using Source.Attributes;

namespace BeforeClassTest
{
    public class BeforeClassTest
    {
        public static int CheckValue { get; private set; } = 0;

        [BeforeClass]
        public static void Increment1() => CheckValue++;

        [BeforeClass]
        public static void Increment2() => CheckValue++;
    }
}
