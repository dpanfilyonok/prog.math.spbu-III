using System;

namespace Source.Attributes
{
    public sealed class TestAttribute : Attribute
    {
        public string Ignore { get; }
        public Type ExpectedException { get; }
        
        public TestAttribute(Type expectedException = null, string ignoreReason = null)
        {
            this.ExpectedException = expectedException;
            this.Ignore = ignoreReason;
        }
    }
}