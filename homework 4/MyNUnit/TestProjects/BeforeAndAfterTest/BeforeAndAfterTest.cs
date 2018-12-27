using System;
using Source.Attributes;

namespace BeforeAndAfterTest
{
    public class BeforeAndAfterTest
    {
        public static int CheckValue { get; private set; } = 0;

        [Before]
        public void IncrementBefore1() => CheckValue++;

        [Before]
        public void IncrementBefore2() => CheckValue++;

        [Test]
        public void VoidTest1() {}

        [Test]
        public void VoidTest2() {}

        [Test]
        public void VoidTest3() {}

        [After]
        public void IncrementAfter1() => CheckValue++;

        [After]
        public void IncrementAfter2() => CheckValue++;
    }
}
