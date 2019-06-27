namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using DataLibrary.Attributes;

    public sealed class ClassMetadata
    {
        public Type Type { get; }

        public IList<MethodMetadata> Methods { get; } = new List<MethodMetadata>();

        // Helper

        public string Namespace => Type.Namespace;

        public string FullName => Type.FullName;

        public string Name => Type.Name;

        public ClassMetadata(Type type)
        {
            if (type.GetCustomAttribute<DaoAttribute>() is null)
            {
                throw new AccessorException($"Type is not supported for generation. type=[{type.FullName}]");
            }

            Type = type;

            foreach (var method in type.GetMethods())
            {
                var attribute = method.GetCustomAttribute<MethodAttribute>(true);
                if (attribute == null)
                {
                    throw new AccessorException($"Method is not supported for generation. type=[{type.FullName}], method=[{method.Name}]");
                }

                Methods.Add(new MethodMetadata(method));
            }
        }

        public AssemblyName[] GetReferencedAssemblies() => Type.Assembly.GetReferencedAssemblies();
    }
}
