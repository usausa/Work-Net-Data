using System;
using System.Data;
using DataLibrary.Engine;
using Smart.ComponentModel;

namespace DataLibrary.Mappers
{
    public interface IResultMapperFactory
    {
        bool IsMatch(Type type);

        Func<IDataRecord, T> CreateMapper<T>(IComponentContainer container, Type type, ColumnInfo[] columns);
    }
}
