namespace DataLibrary.Generator
{
    using System.Reflection;

    public class ParameterMetadata
    {
        public ParameterInfo Parameter { get; }

        public ParameterMetadata(ParameterInfo pi)
        {
            Parameter = pi;
        }
    }
}
