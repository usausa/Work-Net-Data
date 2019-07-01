namespace DataLibrary.Blocks
{
    public sealed class CodeBlock : IBlock
    {
        private readonly string value;

        public CodeBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IBlockContext context) => true;

        public void Build(ICodeBuilder builder) => builder.AddCode(value);
    }
}
