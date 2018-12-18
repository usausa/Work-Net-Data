namespace WorkGenerator.Generators.Misc
{
    public sealed class MethodMetadata
    {
        public AttributeMetadata ExecutorAttribute { get; set; }

        public string ReturnType { get; set; }

        public string Name { get; set; }

        public ParameterMetadata[] Parameters { get; set; }
    }
}
