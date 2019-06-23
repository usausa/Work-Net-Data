namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Blocks;
    using DataLibrary.Loader;
    using DataLibrary.Parser;
    using DataLibrary.Tokenizer;

    public sealed class DirectSqlAttribute : MethodAttribute
    {
        private readonly string sql;

        public DirectSqlAttribute(CommandType commandType, MethodType methodType, string sql)
            : base(commandType, methodType)
        {
            this.sql = sql;
        }

        public override IReadOnlyList<IBlock> CreateTokens(ISqlLoader loader, IBlockParser parser, MethodInfo mi)
        {
            var tokenizer = new SqlTokenizer(sql);
            var tokens = tokenizer.Tokenize();
            return parser.Parse(tokens);
        }
    }
}
