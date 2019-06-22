namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Loader;
    using DataLibrary.Tokenizer;

    public sealed class ProcedureAttribute : MethodAttribute
    {
        private readonly string procedure;

        public ProcedureAttribute(string procedure)
            : base(CommandType.StoredProcedure, MethodType.Execute)
        {
            this.procedure = procedure;
        }

        public override ICollection<Token> CreateTokens(ISqlLoader loader, MethodInfo mi)
        {
            return new[]
            {
                new Token(TokenType.Block, procedure),
            };
        }
    }
}
