namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Loader;
    using DataLibrary.Nodes;
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

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, MethodInfo mi)
        {
            var tokenizer = new SqlTokenizer(sql);
            var tokens = tokenizer.Tokenize();
            var parser = new NodeParser(tokens);
            return parser.Parse();
        }
    }
}
