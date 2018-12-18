namespace WorkGenerator.Attribute
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class TimeoutAttribute : Attribute
    {
    }
}
