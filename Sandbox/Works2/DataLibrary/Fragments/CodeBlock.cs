namespace DataLibrary.Fragments
{
    public sealed class CodeBlock : IFragment
    {
        private readonly string value;

        public CodeBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IFragmentContext context) => true;

        public void Build(IFragmentCodeBuilder builder) => builder.AddCode(value);
    }
}
