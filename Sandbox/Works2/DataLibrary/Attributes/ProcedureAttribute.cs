using System.Linq;
using DataLibrary.Helpers;

namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Nodes;
    using DataLibrary.Loader;

    public sealed class ProcedureAttribute : MethodAttribute
    {
        private readonly string procedure;

        public ProcedureAttribute(string procedure)
            : base(CommandType.StoredProcedure, MethodType.Execute)
        {
            this.procedure = procedure;
        }

        public ProcedureAttribute(string procedure, MethodType methodType)
            : base(CommandType.StoredProcedure, methodType)
        {
            this.procedure = procedure;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, MethodInfo mi)
        {
            var nodes = new List<INode>
            {
                new CodeNode(procedure)
            };

            foreach (var pmi in mi.GetParameters().Where(ParameterHelper.IsSqlParameter))
            {
                if (ParameterHelper.IsNestedParameter(pmi))
                {
                    foreach (var pi in pmi.ParameterType.GetProperties().Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null))
                    {
                        nodes.Add(new ParameterNode(
                            $"{pmi.Name}.{pi.Name}",
                            pi.GetCustomAttribute<ParameterAttribute>()?.Name ?? pi.Name));
                    }
                }
                else
                {
                    nodes.Add(new ParameterNode(
                        pmi.Name,
                        pmi.GetCustomAttribute<ParameterAttribute>()?.Name ?? pmi.Name));
                }
            }

            return nodes;
        }
    }
}
