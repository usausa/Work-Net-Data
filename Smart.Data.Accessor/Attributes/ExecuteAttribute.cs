﻿namespace Smart.Data.Accessor.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExecuteAttribute : Attribute
    {
        public string Text { get; }

        public ExecuteAttribute(string text)
        {
            Text = text;
        }
    }
}
