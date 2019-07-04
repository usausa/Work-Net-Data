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
        private static readonly string[] ParameterNames;

        static RuntimeHelper()
        {
            ParameterNames = Enumerable.Range(0, 256).Select(x => $"p{x}").ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetParameterName(int index)
        {
            return index < ParameterNames.Length ? ParameterNames[index] : $"p{index}";
        }

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

        public static Action<DbCommand, string, StringBuilder, T[]> GetArrayParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateArrayParameterSetup<T>(provider);
        }

        public static Action<DbCommand, string, StringBuilder, IList<T>> GetListParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateListParameterSetup<T>(provider);
        }

        public static Action<DbCommand, string, StringBuilder, IEnumerable<T>> GetEnumerableParameterSetup<T>(ExecuteEngine engine, MethodInfo method, string source)
        {
            var provider = GetCustomAttributeProvider(method, source);
            return engine.CreateEnumerableParameterSetup<T>(provider);
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
