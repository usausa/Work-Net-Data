namespace DataLibrary.Generator
{
    using System.Collections.Generic;
    using System.Linq;

    using DataLibrary.Nodes;

    internal sealed class UsingResolveVisitor : NodeVisitorBase
    {
        private readonly HashSet<string> usings = new HashSet<string>();

        private readonly HashSet<string> helpers = new HashSet<string>();

        public IEnumerable<string> Usings
        {
            get { return usings.OrderBy(x => x); }
        }

        public IEnumerable<string> Helpers
        {
            get { return helpers.OrderBy(x => x); }
        }

        public override void Visit(UsingNode node)
        {
            if (node.IsStatic)
            {
                helpers.Add(node.Name);
            }
            else
            {
                usings.Add(node.Name);
            }
        }
    }
}
