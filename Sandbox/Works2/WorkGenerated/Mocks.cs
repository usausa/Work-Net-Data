using System;
using System.Data;

namespace WorkGenerated
{
    using System.Data.Common;

    public interface IDbProvider
    {
        DbConnection CreateConnection();
    }

    public class DelegateDbProvider : IDbProvider
    {
        private readonly Func<DbConnection> factory;

        public DelegateDbProvider(Func<DbConnection> factory)
        {
            this.factory = factory;
        }

        public DbConnection CreateConnection() => factory();
    }

    public class DataEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class ProcParameter
    {
        public int? InParam { get; set; }

        public int? InOutParam { get; set; }

        public int OutParam { get; set; }
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
