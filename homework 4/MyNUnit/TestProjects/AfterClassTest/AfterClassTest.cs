using System;
using Source.Attributes;

namespace AfterClassTest
{
    public class AfterClassTest
    {
        public static int CheckValue { get; private set; } = 0;

        [AfterClass]
        public void Increment1() => CheckValue++;

        [AfterClass]
        public void Increment2() => CheckValue++;
    }
}
