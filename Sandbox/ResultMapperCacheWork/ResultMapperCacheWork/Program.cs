using System;
using System.Data;

namespace ResultMapperCacheWork
{
    class Program
    {
        static void Main()
        {
        }
    }

    public sealed class Engine
    {
        public Func<IDataRecord, T> CreateFactory<T>()
        {
            return x => Activator.CreateInstance<T>();
        }

        public ResultMapperCache CreateCache() => new ResultMapperCache(this);
    }


    public sealed class ResultMapperCache
    {
        private readonly Engine engine;

        public ResultMapperCache(Engine engine)
        {
            this.engine = engine;
        }

        public Func<IDataRecord, T> ResolveMapper<T>(IDataRecord record)
        {
            // TODO
            return engine.CreateFactory<T>();
        }
    }
}
