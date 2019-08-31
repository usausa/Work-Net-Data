namespace Smart.Data.Accessor.Builders
{
    using System.Reflection;

    public sealed class BuildParameterInfo
    {
        private readonly ParameterInfo parameter;

        private readonly PropertyInfo property;

        public string Name { get; }

        public string ParameterName { get; }

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
    }
}
