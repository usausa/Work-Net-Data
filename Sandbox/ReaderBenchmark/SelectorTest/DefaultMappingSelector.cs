namespace SelectorTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Configs;

    public class DefaultMappingSelector : IMappingSelector
    {
        public TypeMapInfo Select(MethodInfo mi, Type type, ColumnInfo[] columns)
        {
            var columnMap = columns
                .Select((x, i) => new ColumnAndIndex { Column = x, Index = i })
                .ToDictionary(x => x.Column.Name, StringComparer.OrdinalIgnoreCase);

            // Find constructor
            var ctor = type.GetConstructors()
                .Select(x => MatchConstructor(mi, x, columnMap))
                .Where(x => x != null)
                .OrderByDescending(x => x.Map.Indexes.Length)
                .ThenByDescending(x => x.TypeMatch)
                .FirstOrDefault();
            if (ctor == null)
            {
                return null;
            }

            // Remove constructor columns
            foreach (var index in ctor.Map.Indexes)
            {
                columnMap.Remove(columns[index].Name);
            }

            // Gather property map
            var propertyMaps = new List<PropertyMapInfo>();
            foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(IsTargetProperty))
            {
                var name = ConfigHelper.GetMethodPropertyColumnName(mi, pi);
                if (!columnMap.TryGetValue(name, out var column))
                {
                    continue;
                }

                propertyMaps.Add(new PropertyMapInfo(pi, column.Index));
            }

            return new TypeMapInfo(ctor.Map, propertyMaps.ToArray());
        }

        private ConstructorMatch MatchConstructor(MethodInfo mi, ConstructorInfo ci, Dictionary<string, ColumnAndIndex> columnMap)
        {
            var indexes = new List<int>();
            var typeMatch = 0;
            foreach (var pi in ci.GetParameters())
            {
                var name = ConfigHelper.GetMethodParameterColumnName(mi, pi);
                if (!columnMap.TryGetValue(name, out var column))
                {
                    return null;
                }

                indexes.Add(column.Index);
                typeMatch += column.Column.Type == pi.ParameterType ? 1 : 0;
            }

            return new ConstructorMatch
            {
                Map = new ConstructorMapInfo(ci, indexes.OrderBy(x => x).ToArray()),
                TypeMatch = typeMatch
            };
        }

        private static bool IsTargetProperty(PropertyInfo pi)
        {
            return pi.CanWrite && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        }

        private class ColumnAndIndex
        {
            public ColumnInfo Column { get; set; }

            public int Index { get; set; }
        }

        private class ConstructorMatch
        {
            public ConstructorMapInfo Map { get; set; }

            public int TypeMatch { get; set; }
        }
    }
}
