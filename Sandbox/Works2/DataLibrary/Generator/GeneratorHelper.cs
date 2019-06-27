using System.Text;

namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class GeneratorHelper
    {
        public static string MakeGlobalName(Type type)
        {
            var sb = new StringBuilder();
            MakeGlobalName(sb, type);
            return sb.ToString();
        }

        public static void MakeGlobalName(StringBuilder sb, Type type)
        {
            if (type == typeof(void))
            {
                sb.Append("void");
                return;
            }

            if (type.IsGenericType)
            {
                var index = type.Name.IndexOf('`');
                sb.Append("global::").Append(type.Namespace).Append(".").Append(type.Name.Substring(0, index));
                sb.Append("<");

                foreach (var argumentType in type.GetGenericArguments())
                {
                    MakeGlobalName(sb, argumentType);
                }

                sb.Append(">");
            }
            else
            {
                sb.Append("global::").Append(type.Namespace).Append(".").Append(type.Name);
            }
        }

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
