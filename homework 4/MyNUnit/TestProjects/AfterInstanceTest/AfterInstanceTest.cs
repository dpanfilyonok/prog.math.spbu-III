using System;
using Source.Attributes;

namespace AfterInstanceTest
{
    public class AfterInstanceTest
    {
        public static int AfterHash { get; private set; }
        public static int TestHash { get; private set; }

        [After]
        public void After() => AfterHash = this.GetHashCode();

        [Test]
        public void Test() => TestHash = this.GetHashCode();
    }
}
