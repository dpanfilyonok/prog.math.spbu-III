using System;

namespace Source.Attributes
{
    public sealed class TestClassAttribute : Attribute
    {
        public string Ignore { get; }
        public Exception ExpectedException { get; }
        public TestClassAttribute(Exception expectedException, string ignore)
        {
            this.ExpectedException = expectedException;
            this.Ignore = ignore;
        }
    }
}
