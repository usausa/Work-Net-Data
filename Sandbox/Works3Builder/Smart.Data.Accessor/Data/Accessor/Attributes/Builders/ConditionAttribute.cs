namespace Smart.Data.Accessor.Attributes.Builders
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class ConditionAttribute : Attribute
    {
        public string Operand { get; }

        public ConditionAttribute()
            : this(Builders.Operand.Equal)
        {
        }

        public ConditionAttribute(string operand)
        {
            Operand = operand;
        }
    }
}
