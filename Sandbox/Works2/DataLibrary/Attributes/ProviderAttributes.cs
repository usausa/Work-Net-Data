namespace DataLibrary.Attributes
{
    using System;

    using DataLibrary.Providers;

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ProviderAttribute : Attribute
    {
        public Type SelectorType { get; }

        public object Parameter { get; }

        protected ProviderAttribute(Type selectorType, object parameter)
        {
            SelectorType = selectorType;
            Parameter = parameter;
        }
    }
}
