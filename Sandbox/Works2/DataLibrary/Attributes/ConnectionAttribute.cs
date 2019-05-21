namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ConnectionAttribute : Attribute
    {
        public string Name { get; }

        public ConnectionAttribute(string name)
        {
            Name = name;
        }
    }
}
