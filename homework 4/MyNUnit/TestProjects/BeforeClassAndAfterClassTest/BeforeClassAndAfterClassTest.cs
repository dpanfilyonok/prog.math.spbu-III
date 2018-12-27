using System;
using Source.Attributes;

namespace BeforeClassAndAfterClassTest
{
    public class BeforeClassAndAfterClassTest
    {   
        public static int CheckValue { get; private set; } = 0;

        [BeforeClass]
        public void Increment1() => CheckValue++;

        [BeforeClass]
        public void Increment2() => CheckValue++;

        [AfterClass]
        public void Increment3() => CheckValue++;

        [AfterClass]
        public void Increment4() => CheckValue++;
    }
}
