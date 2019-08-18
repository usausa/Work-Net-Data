namespace Smart.Data.Accessor.Generator.Metadata
{
    internal sealed class DynamicParameterEntry
    {
        public string Name { get; }

        public int Index { get; }

        public DynamicParameterEntry(string name, int index)
        {
            Name = name;
            Index = index;
        }
    }
}
