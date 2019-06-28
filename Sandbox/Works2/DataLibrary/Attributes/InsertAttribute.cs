namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Blocks;
    using DataLibrary.Loader;

    public sealed class InsertAttribute : MethodAttribute
    {
        private readonly string table;

        public InsertAttribute(string table)
            : base(CommandType.Text, MethodType.Execute)
        {
            this.table = table;
        }

        public override IReadOnlyList<IBlock> GetBlocks(ISqlLoader loader, MethodInfo mi)
        {
            // TODO object(single and not simple) or parameters!
            var blocks = new List<IBlock>();

            blocks.Add(new SqlBlock($"INSERT INTO {table} ("));
            // TODO
            blocks.Add(new SqlBlock(") VALUES ("));
            // TODO
            blocks.Add(new SqlBlock(")"));

            return blocks;
        }
    }
}
