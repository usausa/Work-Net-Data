namespace DataLibrary.Attributes
{
    using System;

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
