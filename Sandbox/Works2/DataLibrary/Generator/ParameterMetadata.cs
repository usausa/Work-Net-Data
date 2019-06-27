namespace DataLibrary.Generator
{
    using System;
    using System.Reflection;

    public class ParameterMetadata
    {
        public ParameterInfo Parameter { get; }

        public Type Type => Parameter.ParameterType;

        public string Name => Parameter.Name;

        public bool IsRef => Parameter.ParameterType.IsByRef;

        public bool IsOut => Parameter.IsOut;

        public ParameterMetadata(ParameterInfo pi)
        {
            Parameter = pi;
        }
    }
}
