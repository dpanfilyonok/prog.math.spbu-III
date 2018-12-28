using System;
using Source.Attributes;

namespace BeforeClassAndAfterClassTest
{
    public class BeforeClassAndAfterClassTest
    {   
        public static int CheckValue { get; private set; } = 0;

        [BeforeClass]
        public static void Increment1() => CheckValue++;

        [BeforeClass]
        public static void Increment2() => CheckValue++;

        [AfterClass]
        public static void Increment3() => CheckValue++;

        [AfterClass]
        public static void Increment4() => CheckValue++;
    }
}
