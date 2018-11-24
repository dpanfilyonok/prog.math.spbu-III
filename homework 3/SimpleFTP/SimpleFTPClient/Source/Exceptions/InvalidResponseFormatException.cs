namespace Source.Exceptions
{
    [System.Serializable]
    public class InvalidResponseFormatException : System.Exception
    {
        public InvalidResponseFormatException() { }
        public InvalidResponseFormatException(string message) : base(message) { }
        public InvalidResponseFormatException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidResponseFormatException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}