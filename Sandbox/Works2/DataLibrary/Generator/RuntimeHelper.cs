namespace DataLibrary.Generator
{
    using System;
    using System.Linq;
    using System.Reflection;

    using DataLibrary.Attributes;
    using DataLibrary.Engine;
    using DataLibrary.Providers;

    public static class RuntimeHelper
    {
        public static IDbProvider GetDbProvider(ExecuteEngine engine, Type interfaceType)
        {
            var attribute = interfaceType.GetCustomAttribute<ProviderAttribute>();
            var selector = (IDbProviderSelector)engine.Components.Get(attribute.SelectorType);
            return selector.Select(attribute.Parameter);
        }

        public static MethodInfo GetInterfaceMethodByNo(Type type, Type interfaceType, int no)
        {
            var implementMethod = type.GetMethods().First(x => x.GetCustomAttribute<MethodNoAttribute>().No == no);
            var parameterTypes = implementMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            return interfaceType.GetMethod(implementMethod.Name, parameterTypes);
        }
    }
}
