namespace DataLibrary.Generator
{
    using System.Collections.Generic;
    using System.Linq;

    using DataLibrary.Nodes;

    internal sealed class HelperResolveVisitor : NodeVisitorBase
    {
        private readonly HashSet<string> helpers = new HashSet<string>();

        public IEnumerable<string> Helpers
        {
            get { return helpers.OrderBy(x => x); }
        }

        public override void Visit(HelperNode node) => helpers.Add(node.Value);
    }
}
