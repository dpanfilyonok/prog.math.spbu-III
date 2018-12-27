namespace Source.Exceptions
{
    [System.Serializable]
    public class InvalidTestMethodSignatureException : System.Exception
    {
        public InvalidTestMethodSignatureException() { }
        public InvalidTestMethodSignatureException(string message) : base(message) { }
        public InvalidTestMethodSignatureException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidTestMethodSignatureException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
