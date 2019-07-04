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
            // TODO parameter (name, nested)
            return mi.GetParameters()
                .Where(ParameterHelper.IsSqlParameter)
                .Select(x => (INode)new ParameterNode(x.Name, x.Name))
                .Prepend(new CodeNode(procedure))
                .ToList();
        }
    }
}
