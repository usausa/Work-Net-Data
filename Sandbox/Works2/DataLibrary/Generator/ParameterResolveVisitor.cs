using System.Linq;

namespace DataLibrary.Generator
{
    using System.Data;
    using System.Collections.Generic;
    using System.Reflection;

    using DataLibrary.Nodes;

    internal sealed class ParameterResolveVisitor : NodeVisitorBase
    {
        private readonly List<ParameterEntry> entries = new List<ParameterEntry>();

        public IReadOnlyList<ParameterEntry> Entries => entries;

        private readonly MethodInfo method;

        private int index;

        public ParameterResolveVisitor(MethodInfo method)
        {
            this.method = method;
        }

        public override void Visit(ParameterNode node)
        {
            var path = node.Source.Split('.');
            if (path.Length == 1)
            {
                var pi = GetParameterInfo(path[0]);

                // TODO Return 専用ノード？
                entries.Add(new ParameterEntry(
                    node.Source,
                    index++,
                    pi.ParameterType.IsByRef ? pi.ParameterType.GetElementType() : pi.ParameterType,
                    pi.IsOut ? ParameterDirection.Output : pi.ParameterType.IsByRef ? ParameterDirection.InputOutput : ParameterDirection.Input,
                    node.ParameterName));
            }
            else if (path.Length == 2)
            {
                var pi = GetParameterInfo(path[0]);
                // TODO
                entries.Add(new ParameterEntry(
                    node.Source,
                    index++,
                    typeof(string),
                    ParameterDirection.Input,
                    node.ParameterName));
            }
            else
            {
                throw new AccessorGeneratorException("TODO");   // TODO
            }
        }

        private ParameterInfo GetParameterInfo(string name)
        {
            var pi = method.GetParameters().FirstOrDefault(x => x.Name == name);
            if (pi == null)
            {
                throw new AccessorGeneratorException("TODO");   // TODO
            }

            return pi;
        }
    }
}
