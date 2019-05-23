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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExecuteScalarAttribute : Attribute
    {
        public string Source { get; }

        public ExecuteScalarAttribute(string source)
        {
            Source = source;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class QueryAttribute : Attribute
    {
        public string Source { get; }

        public QueryAttribute(string source)
        {
            Source = source;
        }
    }
}
