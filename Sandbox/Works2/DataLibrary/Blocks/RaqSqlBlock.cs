namespace DataLibrary.Blocks
{
    public sealed class RaqSqlBlock : IBlock
    {
        private readonly string value;

        public bool IsDynamic => true;

        public RaqSqlBlock(string value)
        {
            this.value = value;
        }

        public void Process(IBlockContext context) => context.AddRawSql(value);
    }
}
