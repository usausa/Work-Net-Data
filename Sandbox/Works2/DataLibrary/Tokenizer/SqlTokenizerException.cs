namespace DataLibrary.Tokenizer
{
    using System;

    [Serializable]
    public sealed class SqlTokenizerException : AccessorException
    {
        public SqlTokenizerException()
        {
        }

        public SqlTokenizerException(string message)
            : base(message)
        {
        }

        public SqlTokenizerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
