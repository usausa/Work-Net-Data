using System;
using System.Data;
using DataLibrary.Handlers;
using DataLibrary.Providers;

namespace WorkGenerated
{
    using System.Data.Common;

    public class DataEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class ProcParameter
    {
        public string InParam { get; set; }

        public int? InOutParam { get; set; }

        public int OutParam { get; set; }
    }

    public sealed class MockTypeHandler : ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            // TODO
        }

        public object Parse(Type destinationType, object value)
        {
            // TODO
            return value;
        }
    }

    public static class MapperFactory
    {
        public static Func<IDataRecord, DataEntity> CreateDataEntityMapper()
        {
            return r =>
            {
                var entity = new DataEntity();
                entity.Id = r.GetInt32(r.GetOrdinal("Id"));
                entity.Name = r.GetString(r.GetOrdinal("Name"));
                return entity;
            };
        }
    }

    public static class DaoFactory
    {
        public static SampleDao CreateSampleDao(Func<DbConnection> factory)
        {
            return new SampleDao(
                new DelegateDbProvider(factory),
                MapperFactory.CreateDataEntityMapper());
        }
    }
}
