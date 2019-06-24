namespace DataLibrary.Blocks
{
    public sealed class ParameterBlock : IBlock
    {
        private readonly string value;

        public bool IsDynamic => false;

        public ParameterBlock(string value)
        {
            this.value = value;
        }

        public void Process(IBlockContext context) => context.AddParameter(value);
    }
}
