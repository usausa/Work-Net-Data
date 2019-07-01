namespace DataLibrary
{
    using System;

    [Serializable]
    public class AccessorRuntimeException : Exception
    {
        public AccessorRuntimeException()
        {
        }

        public AccessorRuntimeException(string message)
            : base(message)
        {
        }

        public AccessorRuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
