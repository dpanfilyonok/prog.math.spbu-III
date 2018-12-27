using System;
using Source.Attributes;

namespace BeforeInstanceTest
{
    public class BeforeInstanceTest
    {
        public static int BeforeHash { get; private set; }
        public static int TestHash { get; private set; }

        [Before]
        public void Before() => BeforeHash = this.GetHashCode();

        [Test]
        public void Test() => TestHash = this.GetHashCode();
    }
}
