namespace ServerSource.Exceptions
{
    /// <summary>
    /// Throws when connection refused
    /// </summary>
    [System.Serializable]
    public class ConnectionRefusedException : System.Exception
    {
        public ConnectionRefusedException() { }
        public ConnectionRefusedException(string message) : base(message) { }
        public ConnectionRefusedException(string message, System.Exception inner) : base(message, inner) { }
        protected ConnectionRefusedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}