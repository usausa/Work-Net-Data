namespace DataLibrary.Blocks
{
    public sealed class RawSqlBlock : IBlock
    {
        private readonly string value;

        public bool IsDynamic => true;

        public RawSqlBlock(string value)
        {
            this.value = value;
        }

        public void Process(IBlockContext context) => context.AddRawSql(value);
    }
}
