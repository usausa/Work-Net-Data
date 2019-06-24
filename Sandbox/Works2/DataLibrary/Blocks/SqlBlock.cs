namespace DataLibrary.Blocks
{
    public sealed class SqlBlock : IBlock
    {
        private readonly string value;

        public bool IsDynamic => false;

        public SqlBlock(string value)
        {
            this.value = value;
        }

        public void Process(IBlockContext context) => context.AddSql(value);
    }
}
