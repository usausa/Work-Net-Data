namespace WorkGenerator.Metadata
{
    using System;

    public sealed class ClassMetadata
    {
        public string Namespace { get; set; }

        public Attribute Attribute { get; set; }

        public string Name { get; set; }

        public MethodMetadata[] Methods { get; set; }
    }
}
