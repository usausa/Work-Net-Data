﻿namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ParameterAttribute : Attribute
    {
        public string Name { get; }

        public ParameterAttribute(string name)
        {
            Name = name;
        }
    }
}
