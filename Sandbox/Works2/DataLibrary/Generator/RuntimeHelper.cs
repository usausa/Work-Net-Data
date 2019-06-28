namespace DataLibrary.Generator
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class RuntimeHelper
    {
        public static MethodInfo GetInterfaceMethodByNo(Type type, Type interfaceType, int no)
        {
            var implementMethod = type.GetMethods().FirstOrDefault(x => x.GetCustomAttribute<MethodNoAttribute>().No == no);
            var interfaceMap = type.GetInterfaceMap(interfaceType);
            var index = Array.IndexOf(interfaceMap.TargetMethods, implementMethod);
            return interfaceMap.InterfaceMethods[index];
        }
    }
}
