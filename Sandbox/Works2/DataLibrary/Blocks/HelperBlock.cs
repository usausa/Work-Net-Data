namespace DataLibrary.Blocks
{
    public sealed class HelperBlock : IBlock
    {
        private readonly string value;

        public HelperBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IBlockContext context) => false;

        public void Build(ICodeBuilder builder) => builder.AddHelper(value);
    }
}
