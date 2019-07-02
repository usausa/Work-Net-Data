﻿namespace DataLibrary.Attributes
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

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, MethodInfo mi)
        {
            // TODO parameter ?
            return new[]
            {
                new CodeNode(procedure),
            };
        }
    }
}
