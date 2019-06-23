namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Blocks;
    using DataLibrary.Loader;
    using DataLibrary.Parser;

    public sealed class ProcedureAttribute : MethodAttribute
    {
        private readonly string procedure;

        public ProcedureAttribute(string procedure)
            : base(CommandType.StoredProcedure, MethodType.Execute)
        {
            this.procedure = procedure;
        }

        public override IReadOnlyList<IBlock> CreateTokens(ISqlLoader loader, IBlockParser parser, MethodInfo mi)
        {
            return new[]
            {
                new CodeBlock(procedure),
            };
        }
    }
}
