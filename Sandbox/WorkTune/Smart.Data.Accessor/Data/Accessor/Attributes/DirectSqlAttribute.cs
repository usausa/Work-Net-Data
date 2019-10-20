namespace Smart.Data.Accessor.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using Smart.Data.Accessor.Generator;
    using Smart.Data.Accessor.Nodes;
    using Smart.Data.Accessor.Tokenizer;

    public sealed class DirectSqlAttribute : MethodAttribute
    {
        private readonly string sql;

        public DirectSqlAttribute(CommandType commandType, MethodType methodType, string sql)
            : base(commandType, methodType)
        {
            this.sql = sql;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, IGeneratorOption option, MethodInfo mi)
        {
            var tokenizer = new SqlTokenizer(sql);
            var tokens = tokenizer.Tokenize();
            var builder = new NodeBuilder(tokens);
            return builder.Build();
        }
    }
}
