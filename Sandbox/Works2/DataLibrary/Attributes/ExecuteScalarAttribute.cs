namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExecuteScalarAttribute : Attribute
    {
        public string Source { get; }

        public ExecuteScalarAttribute(string source)
        {
            Source = source;
        }
    }
}
