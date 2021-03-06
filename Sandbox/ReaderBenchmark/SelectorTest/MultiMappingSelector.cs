﻿namespace SelectorTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MultiMappingSelector : IMultiMappingSelector
    {
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

                offset = Math.Max(ctor.Indexes.Max(), properties.Select(x => x.Index).Max()) + 1;
            }

            return list.ToArray();
        }
    }
}
