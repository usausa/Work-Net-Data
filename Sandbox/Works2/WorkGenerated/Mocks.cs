using System;
using System.Data;
using DataLibrary.Engine;
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

    public static class DaoFactory
    {
        public static SampleDao CreateSampleDao(Func<DbConnection> factory)
        {
            var config = new ExecuteEngineConfig();
            config.Components.Add<IDbProvider>(new DelegateDbProvider(factory));
            var engine = new ExecuteEngine(config);

            return new SampleDao(engine);
        }
    }
}
