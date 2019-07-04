namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
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

        public static Type MakeInParameterType(Type type)
        {
            var openType = typeof(Action<,,>);
            return openType.MakeGenericType(typeof(DbCommand), typeof(string), type);
        }

        public static Type MakeInOutParameterType(Type type)
        {
            var openType = typeof(Func<,,,>);
            return openType.MakeGenericType(typeof(DbCommand), typeof(string), type, typeof(DbParameter));
        }

        public static Type MakeArrayParameterType(Type type)
        {
            var openType = typeof(Action<,,,>);
            return openType.MakeGenericType(typeof(DbCommand), typeof(string), typeof(StringBuilder), type);
        }

        public static Type MakeListParameterType(Type type)
        {
            var listType = type.GetInterfaces().Prepend(type).First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));
            var openType = typeof(Action<,,,>);
            return openType.MakeGenericType(typeof(DbCommand), typeof(string), typeof(StringBuilder), listType);
        }

        public static Type MakeEnumerableParameterType(Type type)
        {
            var enumerableType = type.GetInterfaces().Prepend(type).First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            var openType = typeof(Action<,,,>);
            return openType.MakeGenericType(typeof(DbCommand), typeof(string), typeof(StringBuilder), enumerableType);
        }

        public static bool IsEnumerable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
        }

        public static bool IsEnumerableParameter(Type type)
        {
            return type != typeof(string) && type.GetInterfaces().Prepend(type).Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static bool IsListParameter(Type type)
        {
            return type.GetInterfaces().Prepend(type).Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static Type GetEnumerableElementType(Type type)
        {
            return type.GetInterfaces().Prepend(type).First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments()[0];
        }

        public static Type GetListElementType(Type type)
        {
            return type.GetInterfaces().Prepend(type).First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>)).GetGenericArguments()[0];
        }
    }
}
