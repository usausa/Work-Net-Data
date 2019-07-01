namespace DataLibrary.Blocks
{
    public sealed class RawSqlBlock : IBlock
    {
        private readonly string value;

        public RawSqlBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IBlockContext context) => true;

        public void Build(ICodeBuilder builder) => builder.AddRawSql(value);
    }
}
