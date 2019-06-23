namespace DataLibrary.Blocks
{
    public sealed class CodeBlock : IBlock
    {
        private readonly string value;

        public bool IsDynamic => true;

        public CodeBlock(string value)
        {
            this.value = value;
        }

        public void Process(IBlockContext context) => context.AddCode(value);
    }
}
