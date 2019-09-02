namespace Smart.Data.Accessor.Attributes.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using Smart.Data.Accessor.Generator;
    using Smart.Data.Accessor.Nodes;

    public sealed class SelectAttribute : MethodAttribute
    {
        private readonly string table;

        private readonly Type type;

        public SelectAttribute()
            : this(null, null)
        {
        }

        public SelectAttribute(string table)
            : this(table, null)
        {
        }

        public SelectAttribute(Type type)
            : this(null, type)
        {
        }

        private SelectAttribute(string table, Type type)
            : base(CommandType.Text, MethodType.Query)
        {
            this.table = table;
            this.type = type;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, IGeneratorOption option, MethodInfo mi)
        {
            throw new NotImplementedException();
        }
    }
}
