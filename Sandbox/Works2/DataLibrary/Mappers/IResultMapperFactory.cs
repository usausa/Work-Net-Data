namespace DataLibrary.Mappers
{
    using System;
    using System.Data;

    using DataLibrary.Engine;

    public interface IResultMapperFactory
    {
        bool IsMatch(Type type);

        Func<IDataRecord, T> CreateMapper<T>(IResultMapperCreateContext context, Type type, ColumnInfo[] columns);
    }
}
