using System;
using Source.Attributes;

namespace BeforeClassTest
{
    public class BeforeClassTest
    {
        public static int CheckValue { get; private set; } = 0;

        [BeforeClass]
        public void Increment1() => CheckValue++;

        [BeforeClass]
        public void Increment2() => CheckValue++;
    }
}
