using System;
using Source.Attributes;

namespace SucceededFailedIgnoredTest
{
    public class SucceededFailedIgnoredTest
    {
        public SucceededFailedIgnoredTest()
        {
        }

        [Test]
        public void Succeeded1() { }

        [Test]
        public void Succeeded2() { }

        [Test(ignoreReason: "test")]
        public void Ignored1() { }

        [Test(ignoreReason: "test")]
        public void Ignored2() => throw new Exception();

        [Test]
        public void Failed1() => throw new Exception();
    }
}
