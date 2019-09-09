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

    public sealed class UpdateAttribute : MethodAttribute
    {
        private readonly string table;

        private readonly Type type;

        public UpdateAttribute()
            : this(null, null)
        {
        }

        public UpdateAttribute(string table)
            : this(table, null)
        {
        }

        public UpdateAttribute(Type type)
            : this(null, type)
        {
        }

        private UpdateAttribute(string table, Type type)
            : base(CommandType.Text, MethodType.Execute)
        {
            this.table = table;
            this.type = type;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, IGeneratorOption option, MethodInfo mi)
        {
            var parameters = BuildHelper.GetParameters(option, mi);
            var values = BuildHelper.GetValueParameters(parameters);

            var sql = new StringBuilder();
            sql.Append("UPDATE ");
            sql.Append(table ?? (type != null ? BuildHelper.GetTableNameOfType(option, type) : null) ?? BuildHelper.GetTableName(option, mi));
            sql.Append(" SET");

            if (values.Count > 0)
            {
                AddSetValues(sql, values);
                BuildHelper.AddCondition(sql, BuildHelper.GetNonValueParameters(parameters));
            }
            else
            {
                var keys = BuildHelper.GetKeyParameters(parameters);
                if (keys.Count > 0)
                {
                    AddSetValues(sql, BuildHelper.GetNonKeyParameters(parameters));
                    BuildHelper.AddCondition(sql, keys);
                }
                else
                {
                    AddSetValues(sql, BuildHelper.GetNonConditionParameters(parameters));
                    BuildHelper.AddCondition(sql, BuildHelper.GetConditionParameters(parameters));
                }
            }

            var tokenizer = new SqlTokenizer(sql.ToString());
            var builder = new NodeBuilder(tokenizer.Tokenize());
            return builder.Build();
        }

        private static void AddSetValues(StringBuilder sql, IReadOnlyList<BuildParameterInfo> parameters)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                if (i > 0)
                {
                    sql.Append(",");
                }

                sql.Append($" {parameter.ParameterName} = ");
                BuildHelper.AddParameter(sql, parameter, Operation.Update);
            }
        }
    }
}
