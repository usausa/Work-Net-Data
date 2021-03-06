namespace Smart.Data.Accessor.Selectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Accessor.Engine;

    public class MultiMappingSelector : IMultiMappingSelector
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public TypeMapInfo[] Select(MethodInfo mi, Type[] types, ColumnInfo[] columns)
        {
            var list = new List<TypeMapInfo>();
            var offset = 0;
            foreach (var type in types)
            {
                var matcher = new ColumnMatcher(mi, columns.Skip(offset), offset);
                var ctor = matcher.ResolveConstructor(type);
                if (ctor is null)
                {
                    return null;
                }

                var properties = matcher.ResolveProperties(type);
                list.Add(new TypeMapInfo(ctor, properties));

                var maxIndex = ctor.Parameters
                    .Select(x => x.Index)
                    .Concat(properties.Select(x => x.Index))
                    .DefaultIfEmpty(-1)
                    .Max();
                if (maxIndex >= 0)
                {
                    offset = maxIndex + 1;
                }
            }

            return list.ToArray();
        }
    }
}
