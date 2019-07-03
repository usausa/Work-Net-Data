namespace DataLibrary.Generator
{
    using System.Data;
    using System.Collections.Generic;

    using DataLibrary.Nodes;

    internal sealed class ParameterResolveVisitor : NodeVisitorBase
    {
        private readonly List<ParameterEntry> entries = new List<ParameterEntry>();

        public IReadOnlyList<ParameterEntry> Entries => entries;

        // TODO ctor

        public override void Visit(ParameterNode node)
        {
            entries.Add(new ParameterEntry(
                "code",
                typeof(string),
                ParameterDirection.Input,
                0));
        }
    }
}
