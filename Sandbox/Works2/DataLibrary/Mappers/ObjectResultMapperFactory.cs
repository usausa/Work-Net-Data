namespace DataLibrary.Mappers
{
    using System;
    using System.Data;

    using DataLibrary.Engine;

    using Smart.Reflection;

    public sealed class ObjectResultMapperFactory : IResultMapperFactory
    {
        public static ObjectResultMapperFactory Instance { get; } = new ObjectResultMapperFactory();

        private ObjectResultMapperFactory()
        {
        }

        public bool IsMatch(Type type) => true;

        public Func<IDataRecord, T> CreateMapper<T>(ExecuteConfig config, Type type, ColumnInfo[] columns)
        {
            var delegateFactory = config.GetComponent<IDelegateFactory>();
            var factory = delegateFactory.CreateFactory<T>();

            // TODO
            //    var entries = CreateMapEntries(config, type, columns);

            return record =>
            {
                var obj = factory();

                // TODO
                //        for (var i = 0; i < entries.Length; i++)
                //        {
                //            var entry = entries[i];
                //            entry.Setter(obj, record.GetValue(entry.Index));
                //        }

                return obj;
            };
        }

        //private static MapEntry[] CreateMapEntries(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
        //{
        //    var selector = config.GetPropertySelector();
        //    var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
        //        .Where(IsTargetProperty)
        //        .ToArray();

        //    var list = new List<MapEntry>();
        //    for (var i = 0; i < columns.Length; i++)
        //    {
        //        var column = columns[i];
        //        var pi = selector(properties, column.Name);
        //        if (pi == null)
        //        {
        //            continue;
        //        }
        //        var setter = config.CreateSetter(pi);
        //        var defaultValue = pi.PropertyType.GetDefaultValue();

        //        if ((pi.PropertyType == column.Type) ||
        //            (pi.PropertyType.IsNullableType() && (Nullable.GetUnderlyingType(pi.PropertyType) == column.Type)))
        //        {
        //            list.Add(new MapEntry(i, (obj, value) => setter(obj, value is DBNull ? defaultValue : value)));
        //        }
        //        else
        //        {
        //            var parser = config.CreateParser(column.Type, pi.PropertyType);
        //            list.Add(new MapEntry(i, (obj, value) => setter(obj, parser(value is DBNull ? defaultValue : value))));
        //        }
        //    }

        //    return list.ToArray();
        //}

        //private static bool IsTargetProperty(PropertyInfo pi)
        //{
        //    return pi.CanWrite && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        //}

        //private sealed class MapEntry
        //{
        //    public int Index { get; }

        //    public Action<object, object> Setter { get; }

        //    public MapEntry(int index, Action<object, object> setter)
        //    {
        //        Index = index;
        //        Setter = setter;
        //    }
        //}
    }
}
