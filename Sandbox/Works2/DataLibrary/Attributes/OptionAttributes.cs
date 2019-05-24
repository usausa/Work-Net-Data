namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class CommandTimeoutAttribute : Attribute
    {
        public int Timeout { get; }

        public CommandTimeoutAttribute(int timeout)
        {
            Timeout = timeout;
        }
    }
}
