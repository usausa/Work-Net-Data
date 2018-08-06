namespace Smart.Data.Accessor
{
    using System;

    [Serializable]
    public class AccessorException : Exception
    {
        public AccessorException()
        {
        }

        public AccessorException(string message)
            : base(message)
        {
        }

        public AccessorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
