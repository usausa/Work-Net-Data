using Smart.ComponentModel;

namespace DataLibrary.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using DataLibrary.Attributes;
    using DataLibrary.Engine;
    using DataLibrary.Selectors;

    using Smart;
    using Smart.Converter;
    using Smart.Reflection;

    public sealed class ObjectResultMapperFactory : IResultMapperFactory
    {
        public static ObjectResultMapperFactory Instance { get; } = new ObjectResultMapperFactory();

        private ObjectResultMapperFactory()
        {
        }

        public bool IsMatch(Type type) => true;

        public Func<IDataRecord, T> CreateMapper<T>(IComponentContainer container, Type type, ColumnInfo[] columns)
        {
            var delegateFactory = container.Get<IDelegateFactory>();
            var objectConverter = container.Get<IObjectConverter>();
            var propertySelector = container.Get<IPropertySelector>();

            var factory = delegateFactory.CreateFactory<T>();
            var entries = CreateMapEntries(delegateFactory, objectConverter, propertySelector, type, columns);

            return record =>
            {
                var obj = factory();

                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                     entry.Setter(obj, record.GetValue(entry.Index));
                }

                return obj;
            };
        }

        private static MapEntry[] CreateMapEntries(
            IDelegateFactory delegateFactory,
            IObjectConverter objectConverter,
            IPropertySelector propertySelector,
            Type type,
            ColumnInfo[] columns)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(IsTargetProperty)
                .ToArray();

            var list = new List<MapEntry>();
            for (var i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                var pi = propertySelector.SelectProperty(properties, column.Name);
                if (pi == null)
                {
                    continue;
                }

                var setter = delegateFactory.CreateSetter(pi);
                var defaultValue = pi.PropertyType.GetDefaultValue();

                if ((pi.PropertyType == column.Type) ||
                    (pi.PropertyType.IsNullableType() && (Nullable.GetUnderlyingType(pi.PropertyType) == column.Type)))
                {
                    list.Add(new MapEntry(i, (obj, value) => setter(obj, value is DBNull ? defaultValue : value)));
                }
                else
                {
                    var converter = objectConverter.CreateConverter(column.Type, pi.PropertyType);
                    list.Add(new MapEntry(i, (obj, value) => setter(obj, converter(value is DBNull ? defaultValue : value))));
                }
            }

            return list.ToArray();
        }

        private static bool IsTargetProperty(PropertyInfo pi)
        {
            return pi.CanWrite && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        }

        private sealed class MapEntry
        {
            public int Index { get; }

            public Action<object, object> Setter { get; }

            public MapEntry(int index, Action<object, object> setter)
            {
                Index = index;
                Setter = setter;
            }
        }
    }
}
