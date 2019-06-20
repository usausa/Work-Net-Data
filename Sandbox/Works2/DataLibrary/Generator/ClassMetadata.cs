namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;

    public sealed class ClassMetadata
    {
        public Type Type { get; }

        public IList<MethodMetadata> Methods { get; } = new List<MethodMetadata>();

        public ClassMetadata(Type type)
        {
            Type = type;
        }
    }
}
