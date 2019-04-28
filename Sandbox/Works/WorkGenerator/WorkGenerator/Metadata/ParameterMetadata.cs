namespace WorkGenerator.Metadata
{
    using System;

    public sealed class ParameterMetadata
    {
        public Attribute[] Attributes { get; set; }

        public Type ParemeterType { get; set; }

        public string Name { get; set; }
    }
}
