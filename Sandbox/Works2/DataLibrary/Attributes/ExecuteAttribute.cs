namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExecuteAttribute : Attribute
    {
        public string Source { get; }

        public ExecuteAttribute(string source)
        {
            Source = source;
        }
    }
}
