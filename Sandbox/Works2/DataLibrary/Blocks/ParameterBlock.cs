namespace DataLibrary.Blocks
{
    public sealed class ParameterBlock : IBlock
    {
        private readonly string value;

        public ParameterBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IBlockContext context) => context.IsDynamicParameter(value);

        public void Build(ICodeBuilder builder) => builder.AddParameter(value);
    }
}
