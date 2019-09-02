namespace Smart.Data.Accessor.Attributes.Builders
{
    using System;
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

        private readonly Type type;

        public CountAttribute()
            : this(null, null)
        {
        }

        public CountAttribute(string table)
            : this(table, null)
        {
        }

        public CountAttribute(Type type)
            : this(null, type)
        {
        }

        private CountAttribute(string table, Type type)
            : base(CommandType.Text, MethodType.ExecuteScalar)
        {
            this.table = table;
            this.type = type;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, IGeneratorOption option, MethodInfo mi)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT COUNT(*) FROM ");
            sql.Append(table ?? (type != null ? BuildHelper.GetTableNameOfType(option, type) : null) ?? BuildHelper.GetTableName(option, mi));
            sql.Append(" WHERE ");
            BuildHelper.AddConditionNode(sql, BuildHelper.GetParameters(option, mi));

            var tokenizer = new SqlTokenizer(sql.ToString());
            var builder = new NodeBuilder(tokenizer.Tokenize());
            return builder.Build();
        }
    }
}
