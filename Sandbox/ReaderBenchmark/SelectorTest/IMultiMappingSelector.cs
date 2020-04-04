namespace SelectorTest
{
    using System;
    using System.Reflection;

    public interface IMultiMappingSelector
    {
        TypeMapInfo[] Select(MethodInfo mi, Type[] types, ColumnInfo[] columns);
    }
}
