namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GeneratorHelper
    {
        public static bool IsListType(Type type)
        {
            return type.GetInterfaces().Prepend(type)
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static bool IsEnumerableType(Type type)
        {
            return type.GetInterfaces().Prepend(type)
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static Type GetGenericElementType(Type type)
        {
            return type.GetGenericArguments()[0];
        }
    }
}
