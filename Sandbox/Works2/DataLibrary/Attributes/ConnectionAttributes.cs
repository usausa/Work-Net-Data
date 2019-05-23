namespace DataLibrary.Attributes
{
    using System;

    using DataLibrary.Connections;

    public interface IConnectionFactoryAttribute
    {
        Type FactoryType { get; }

        // TODO generate source ?
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ConnectionNameAttribute : Attribute, IConnectionFactoryAttribute
    {
        public string Name { get; }

        public Type FactoryType { get; } = typeof(NamedConnectionFactory);

        public ConnectionNameAttribute(string name)
        {
            Name = name;
        }
    }
}
