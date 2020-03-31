namespace SelectorTest
{
    using System;

    public interface IMappingSelector
    {
        TypeMapInfo Select(Type type, ColumnInfo[] columns);
    }
}
