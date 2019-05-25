namespace DataLibrary.Engine
{
    using System;

    [Serializable]
    public class EngineException : AccessorException
    {
        public EngineException()
        {
        }

        public EngineException(string message)
            : base(message)
        {
        }

        public EngineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
