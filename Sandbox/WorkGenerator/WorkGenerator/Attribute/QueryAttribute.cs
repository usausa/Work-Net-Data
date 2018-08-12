namespace WorkGenerator.Attribute
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class QueryAttribute : Attribute
    {
        public string Text { get; }

        public QueryAttribute(string text)
        {
            Text = text;
        }
    }
}
