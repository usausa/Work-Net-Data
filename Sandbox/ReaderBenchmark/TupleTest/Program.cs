using System;
using System.Linq;
using System.Reflection;
using Smart.Data.Accessor.Engine;
using Smart.Data.Accessor.Mappers;
using Smart.Data.Accessor.Selectors;
using Smart.Mock.Data;

namespace TupleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var colmuns = new ColumnInfo[]
            {
                new ColumnInfo("Id", typeof(int)),
                new ColumnInfo("Name", typeof(string)),
                new ColumnInfo("Id", typeof(int)),
                new ColumnInfo("Name", typeof(string))
            };

            var reader = new MockDataReader(
                colmuns.Select(x => new MockColumn(x.Type, x.Name)).ToArray(),
                new []
                {
                    new object[] { 1, "Master-1", 101, "Slave1" },
                    new object[] { 1, "Master-1", 102, "Slave2" },
                    new object[] { DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value }
                });

            var context = new DummyContext();
            var mi = typeof(Program).GetMethod(nameof(Main));

            var tupleMapper = TupleResultMapperFactory.Instance.CreateMapper<Tuple<Master, Slave>>(context, mi, colmuns);

            reader.Read();
            var t1 = tupleMapper(reader);
        }
    }

    public class DummyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IMultiMappingSelector))
            {
                return new MultiMappingSelector();
            }

            return null;
        }
    }

    public class DummyContext : IResultMapperCreateContext
    {
        public IServiceProvider ServiceProvider => new DummyServiceProvider();

        public Func<object, object> GetConverter(Type sourceType, Type destinationType, ICustomAttributeProvider provider)
        {
            if (sourceType == destinationType)
            {
                return null;
            }

            throw new NotImplementedException();
        }
    }

    public class Master
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Slave
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
