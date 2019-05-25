namespace DataLibrary.Attributes
{
    using System;

    using DataLibrary.Providers;

    public interface IProviderFactoryAttribute
    {
        Type ProviderType { get; }

        // TODO generate source ?
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProviderNameAttribute : Attribute, IProviderFactoryAttribute
    {
        public string Name { get; }

        public Type ProviderType { get; } = typeof(INamedDbProviderFactory);

        public ProviderNameAttribute(string name)
        {
            Name = name;
        }
    }
}
