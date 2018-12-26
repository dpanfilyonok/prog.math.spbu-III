using System;

namespace Source.Attributes
{
    public sealed class TestAttribute : Attribute
    {
        public string Ignore { get; }
        public Exception ExpectedException { get; }
        public TestAttribute(Exception expectedException = null, string ignoreReason = null)
        {
            this.ExpectedException = expectedException;
            this.Ignore = ignoreReason;
        }
    }
}