namespace WorkGenerator.Metadata
{
    using System;

    public sealed class MethodMetadata
    {
        public Attribute ExecutorAttribute { get; set; }

        public Type ReturnType { get; set; }

        public string Name { get; set; }

        public ParameterMetadata[] Parameters { get; set; }
    }
}
