namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using DataLibrary.Helpers;
    using DataLibrary.Loader;
    using DataLibrary.Nodes;

    public sealed class ProcedureAttribute : MethodAttribute, IReturnValueBehavior
    {
        private readonly string procedure;

        public bool ReturnValueAsResult { get; }

        public ProcedureAttribute(string procedure)
            : this(procedure, MethodType.Execute)
        {
        }

        public ProcedureAttribute(string procedure, MethodType methodType)
            : this(procedure, methodType, true)
        {
        }

        public ProcedureAttribute(string procedure, bool returnValueAsResult)
            : this(procedure, MethodType.Execute, returnValueAsResult)
        {
        }

        private ProcedureAttribute(string procedure, MethodType methodType, bool returnValueAsResult)
            : base(CommandType.StoredProcedure, methodType)
        {
            this.procedure = procedure;
            ReturnValueAsResult = returnValueAsResult;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, MethodInfo mi)
        {
            var nodes = new List<INode>
            {
                new SqlNode(procedure)
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
