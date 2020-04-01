namespace SelectorTest
{
    using System;
    using System.Reflection;

    public interface IMappingSelector
    {
        TypeMapInfo Select(MethodInfo mi, Type type, ColumnInfo[] columns);
    }
}
