namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Loader;
    using DataLibrary.Nodes;

    public sealed class InsertAttribute : MethodAttribute
    {
        private readonly string table;

        public InsertAttribute(string table)
            : base(CommandType.Text, MethodType.Execute)
        {
            this.table = table;
        }

        public override IReadOnlyList<INode> GetNodes(ISqlLoader loader, MethodInfo mi)
        {
            // TODO object(single and not simple) or parameters!
            var nodes = new List<INode>();

            nodes.Add(new SqlNode($"INSERT INTO {table} ("));
            // TODO
            nodes.Add(new SqlNode(") VALUES ("));
            // TODO
            nodes.Add(new SqlNode(")"));

            return nodes;
        }
    }
}
