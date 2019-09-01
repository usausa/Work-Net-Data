namespace Smart.Data.Accessor.Attributes.Builders
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Text;

    using Smart.Data.Accessor.Attributes.Builders.Helpers;
    using Smart.Data.Accessor.Generator;
    using Smart.Data.Accessor.Nodes;
    using Smart.Data.Accessor.Tokenizer;

    public sealed class CountAttribute : MethodAttribute
    {
        private readonly string table;

        public CountAttribute()
            : this(null)
        {
        }

        public CountAttribute(string table)
            : base(CommandType.Text, MethodType.ExecuteScalar)
        {
            this.table = table;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, IGeneratorOption option, MethodInfo mi)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) FROM ");
            sql.Append(table ?? BuildHelper.GetTableName(option, mi));
            sql.Append(" WHERE ");
            BuildHelper.AddConditionNode(sql, BuildHelper.GetParameters(option, mi));

            var tokenizer = new SqlTokenizer(sql.ToString());
            var builder = new NodeBuilder(tokenizer.Tokenize());
            return builder.Build();
        }
    }
}
