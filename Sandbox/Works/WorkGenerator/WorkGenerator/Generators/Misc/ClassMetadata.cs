namespace WorkGenerator.Generators.Misc
{
    public sealed class ClassMetadata
    {
        public string Namespace { get; set; }

        public AttributeMetadata Attribute { get; set; }

        public string Name { get; set; }

        public MethodMetadata[] Methods { get; set; }
    }
}
