namespace Smart.Data.Accessor.Engine
{
    using System;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Ignore")]
    public readonly struct ColumnInfo
    {
        public readonly string Name;

        public readonly Type Type;

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
