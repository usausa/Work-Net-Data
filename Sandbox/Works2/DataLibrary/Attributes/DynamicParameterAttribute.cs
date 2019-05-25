namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class DynamicParameterAttribute : Attribute
    {
        public string Name { get; }

        public DynamicParameterAttribute(string name)
        {
            Name = name;
        }

        // TODO size, parameter or Handler
    }
}
