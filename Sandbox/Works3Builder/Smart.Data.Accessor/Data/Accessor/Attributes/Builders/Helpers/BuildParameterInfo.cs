namespace Smart.Data.Accessor.Attributes.Builders.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class BuildParameterInfo
    {
        private readonly ParameterInfo parameter;

        private readonly PropertyInfo property;

        public string Name { get; }

        public string ParameterName { get; }

        public Type ParameterType => parameter != null ? parameter.ParameterType : property.PropertyType;

        public BuildParameterInfo(ParameterInfo parameter, string name, string parameterName)
        {
            this.parameter = parameter;
            this.property = null;
            Name = name;
            ParameterName = parameterName;
        }

        public BuildParameterInfo(PropertyInfo property, string name, string parameterName)
        {
            this.parameter = null;
            this.property = property;
            Name = name;
            ParameterName = parameterName;
        }

        public T GetCustomAttribute<T>()
            where T : Attribute
        {
            return parameter != null
                ? parameter.GetCustomAttribute<T>()
                : property.GetCustomAttribute<T>();
        }

        public IEnumerable<T> GetCustomAttributes<T>()
            where T : Attribute
        {
            return parameter != null
                ? parameter.GetCustomAttributes<T>()
                : property.GetCustomAttributes<T>();
        }
    }
}
