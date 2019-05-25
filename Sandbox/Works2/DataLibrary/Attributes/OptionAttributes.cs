namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TimeoutAttribute : Attribute
    {
        public int Timeout { get; }

        public TimeoutAttribute(int timeout)
        {
            Timeout = timeout;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class TimeoutParameterAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class BufferedAttribute : Attribute
    {
        public bool Buffered { get; }

        public BufferedAttribute(bool buffered)
        {
            Buffered = buffered;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class BufferedParameterAttribute : Attribute
    {
    }
}
