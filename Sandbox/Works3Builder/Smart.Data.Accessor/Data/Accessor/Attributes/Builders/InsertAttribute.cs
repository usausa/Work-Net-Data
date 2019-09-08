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

    public sealed class InsertAttribute : MethodAttribute
    {
        private readonly string table;

        private readonly Type type;

        public InsertAttribute()
            : this(null, null)
        {
        }

        public InsertAttribute(string table)
            : this(table, null)
        {
        }

        public InsertAttribute(Type type)
            : this(null, type)
        {
        }

        private InsertAttribute(string table, Type type)
            : base(CommandType.Text, MethodType.Execute)
        {
            this.table = table;
            this.type = type;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, IGeneratorOption option, MethodInfo mi)
        {
            var parameters = BuildHelper.GetParameters(option, mi);

            var sql = new StringBuilder();
            sql.Append("INSERT INTO ");
            sql.Append(table ?? (type != null ? BuildHelper.GetTableNameOfType(option, type) : null) ?? BuildHelper.GetTableName(option, mi));
            sql.Append(" (");

            for (var i = 0; i < parameters.Count; i++)
            {
                if (i != 0)
                {
                    sql.Append(", ");
                }

                sql.Append(parameters[i].ParameterName);
            }

            sql.Append(") VALUES (");

            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                if (i != 0)
                {
                    sql.Append(", ");
                }

                BuildHelper.AddParameter(sql, parameter, Operation.Insert);
            }

            sql.Append(")");

            var tokenizer = new SqlTokenizer(sql.ToString());
            var builder = new NodeBuilder(tokenizer.Tokenize());
            return builder.Build();
        }
    }
}
