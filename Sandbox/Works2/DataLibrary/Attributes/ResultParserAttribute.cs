namespace DataLibrary.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Method)]
    public abstract class ResultParserAttribute : Attribute
    {
        public abstract Func<object, object> CreateConverter(Type type);
    }
}
