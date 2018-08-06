namespace Smart.Data.Accessor.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExecuteReaderAttribute : Attribute, IExecutorAttribute
    {
        public string Text { get; }

        public ExecuteReaderAttribute(string text)
        {
            Text = text;
        }
    }
}
