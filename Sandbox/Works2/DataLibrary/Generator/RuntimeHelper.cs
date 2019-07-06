using System.Collections.Generic;

namespace DataLibrary.Generator
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using DataLibrary.Attributes;
    using DataLibrary.Engine;
    using DataLibrary.Providers;

    public static class RuntimeHelper
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Convert<T>(object source, Func<object, object> converter)
        {
            if (source is T value)
            {
                return value;
            }

            if (source is DBNull)
            {
                return default;
            }

            return (T)converter(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Convert<T>(DbParameter parameter, Func<object, object> converter)
        {
            if (parameter is null)
            {
                return default;
            }

            var source = parameter.Value;

            if (source is T value)
            {
                return value;
            }

            if (source is DBNull)
            {
                return default;
            }

            return (T)converter(source);
        }

        //--------------------------------------------------------------------------------
        // Initialize
        //--------------------------------------------------------------------------------

        public static MethodInfo GetInterfaceMethodByNo(Type type, Type interfaceType, int no)
        {
            var implementMethod = type.GetMethods().First(x => x.GetCustomAttribute<MethodNoAttribute>().No == no);
            var parameterTypes = implementMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            return interfaceType.GetMethod(implementMethod.Name, parameterTypes);
        }

        public static IDbProvider GetDbProvider(ExecuteEngine engine, Type interfaceType)
        {
            var attribute = interfaceType.GetCustomAttribute<ProviderAttribute>();
            var selector = (IDbProviderSelector)engine.Components.Get(attribute.SelectorType);
            return selector.Select(attribute.Parameter);
        }

        public static IDbProvider GetDbProvider(ExecuteEngine engine, MethodInfo method)
        {
            var attribute = method.GetCustomAttribute<ProviderAttribute>();
            var selector = (IDbProviderSelector)engine.Components.Get(attribute.SelectorType);
            return selector.Select(attribute.Parameter);
        }

        private static ICustomAttributeProvider GetCustomAttributeProvider(MethodInfo method, string source)
        {
            var path = source.Split('.');
            var parameter = method.GetParameters().First(x => x.Name == path[0]);
            if (path.Length == 1)
            {
                return parameter;
            }

            return parameter.ParameterType.GetProperty(path[1]);
        }

        public static Action<DbCommand, string, T[]> GetArrayParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateArrayParameterSetup<T>(provider);
        }

        public static Action<DbCommand, string, IList<T>> GetListParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateListParameterSetup<T>(provider);
        }

        public static Action<DbCommand, string, T> GetInParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateInParameterSetup<T>(provider);
        }

        public static Func<DbCommand, string, T, DbParameter> GetInOutParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateInOutParameterSetup<T>(provider);
        }

        public static Func<DbCommand, string, DbParameter> GetOutParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateOutParameterSetup<T>(provider);
        }

        public static Func<object, object> GetConverter<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateConverter<T>(provider);
        }
    }
}
