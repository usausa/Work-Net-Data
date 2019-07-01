namespace DataLibrary.Fragments
{
    public sealed class HelperBlock : IFragment
    {
        private readonly string value;

        public HelperBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IFragmentContext context) => false;

        public void Build(IFragmentCodeBuilder builder) => builder.AddHelper(value);
    }
}
