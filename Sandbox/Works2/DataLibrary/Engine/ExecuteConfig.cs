namespace DataLibrary.Engine
{
    using System;
    using System.Data;

    using DataLibrary.Mappers;

    using Smart.Collections.Concurrent;

    // TODO interface ?
    public class ExecuteConfig
    {
        private readonly ThreadsafeTypeHashArrayMap<object> components = new ThreadsafeTypeHashArrayMap<object>();

        public T GetComponent<T>()
        {
            // TODO
            if (components.TryGetValue(typeof(T), out var component))
            {
                return (T)component;
            }

            return Activator.CreateInstance<T>();
        }

        public void AddComponent<T>(T component)
        {
            components.AddIfNotExist(typeof(T), component);
        }

        [ThreadStatic]
        private static ColumnInfo[] columnInfoPool;

        private readonly ResultMapperCache resultMapperCache = new ResultMapperCache();

        private readonly IResultMapperFactory[] resultMapperFactories =
        {
            ObjectResultMapperFactory.Instance
        };

        public Func<IDataRecord, T> CreateResultMapper<T>(IDataReader reader)
        {
            var fieldCount = reader.FieldCount;
            if ((columnInfoPool == null) || (columnInfoPool.Length < fieldCount))
            {
                columnInfoPool = new ColumnInfo[fieldCount];
            }

            var type = typeof(T);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                columnInfoPool[i] = new ColumnInfo(reader.GetName(i), reader.GetFieldType(i));
            }

            var columns = new Span<ColumnInfo>(columnInfoPool, 0, fieldCount);

            if (resultMapperCache.TryGetValue(type, columns, out var value))
            {
                return (Func<IDataRecord, T>)value;
            }

            return (Func<IDataRecord, T>)resultMapperCache.AddIfNotExist(type, columns, CreateMapperInternal<T>);
        }

        private object CreateMapperInternal<T>(Type type, ColumnInfo[] columns)
        {
            foreach (var factory in resultMapperFactories)
            {
                if (factory.IsMatch(type))
                {
                    return factory.CreateMapper<T>(type, columns);
                }
            }

            throw new AccessorException($"Result type is not supported. type=[{type.FullName}]");
        }
    }
}
