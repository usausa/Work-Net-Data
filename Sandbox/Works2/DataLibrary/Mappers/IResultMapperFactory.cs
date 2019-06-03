using System;
using System.Data;
using DataLibrary.Engine;

namespace DataLibrary.Mappers
{
    public interface IResultMapperFactory
    {
        bool IsMatch(Type type);

        Func<IDataRecord, T> CreateMapper<T>(Type type, ColumnInfo[] columns);
    }
}
