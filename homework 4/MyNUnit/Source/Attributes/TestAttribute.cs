using System;

namespace Source.Attributes
{
    public sealed class TestAttribute : Attribute
    {
        public string Ignore { get; }
        public Exception ExpectedException { get; }
        public TestAttribute(Exception expectedException, string ignore)
        {
            this.ExpectedException = expectedException;
            this.Ignore = ignore;
        }
    }
}