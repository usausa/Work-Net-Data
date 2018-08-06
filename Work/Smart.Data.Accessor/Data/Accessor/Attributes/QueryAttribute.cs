﻿namespace Smart.Data.Accessor.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class QueryAttribute : Attribute, IExecutorAttribute
    {
        public string Text { get; }

        public QueryAttribute(string text)
        {
            Text = text;
        }
    }
}
