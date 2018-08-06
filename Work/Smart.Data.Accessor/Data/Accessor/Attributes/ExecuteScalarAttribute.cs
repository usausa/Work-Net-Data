namespace Smart.Data.Accessor.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExecuteScalarAttribute : Attribute, IExecutorAttribute
    {
        public string Text { get; }

        public ExecuteScalarAttribute(string text)
        {
            Text = text;
        }
    }
}
