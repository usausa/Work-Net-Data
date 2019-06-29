namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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

                var first = true;
                foreach (var argumentType in type.GetGenericArguments())
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    MakeGlobalName(sb, argumentType);
                }

                sb.Append(">");
            }
            else
            {
                sb.Append("global::").Append(type.Namespace).Append(".").Append(type.Name);
            }
        }

        public static bool IsEnumerableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool IsListType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
        }

        public static Type GetElementType(Type type)
        {
            return type.GetGenericArguments()[0];
        }
    }
}
