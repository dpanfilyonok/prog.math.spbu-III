using System;
using Source.Attributes;

namespace ExpectedExceptionTest
{
    public class ExpectedExceptionTest
    {
        [Test(expectedException: typeof(NullReferenceException))]
        public void CorrectException() => throw new NullReferenceException();

        [Test(expectedException: typeof(NullReferenceException))]
        public void IncorrectException() => throw new ArgumentException();
    }
}
