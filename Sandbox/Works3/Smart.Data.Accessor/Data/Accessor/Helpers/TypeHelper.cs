﻿namespace Smart.Data.Accessor.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class TypeHelper
    {
        //--------------------------------------------------------------------------------
        // For result type
        //--------------------------------------------------------------------------------

        public static bool IsEnumerable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>);
        }

        //--------------------------------------------------------------------------------
        // For parameter type
        //--------------------------------------------------------------------------------

        public static bool IsEnumerableParameter(Type type)
        {
            return (type != typeof(string)) && (type != typeof(byte[])) && type.GetInterfaces().Prepend(type).Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static bool IsArrayParameter(Type type)
        {
            return (type != typeof(byte[])) && type.IsArray;
        }

        public static bool IsListParameter(Type type)
        {
            return type.GetInterfaces().Prepend(type).Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));
        }

        //--------------------------------------------------------------------------------
        // For parameter element
        //--------------------------------------------------------------------------------

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