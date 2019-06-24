namespace DataLibrary.Blocks
{
    public sealed class ImportBlock : IBlock
    {
        private readonly string value;

        private readonly bool isStatic;

        public bool IsDynamic => false;

        public ImportBlock(string value, bool isStatic)
        {
            this.value = value;
            this.isStatic = isStatic;
        }

        public void Process(IBlockContext context) => context.AddImport(value, isStatic);
    }
}
