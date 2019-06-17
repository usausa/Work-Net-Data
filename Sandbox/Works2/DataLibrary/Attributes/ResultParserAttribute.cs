namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public abstract class ResultAttribute : Attribute
    {
        public abstract Func<object, object> CreateConverter(Type type);
    }
}
