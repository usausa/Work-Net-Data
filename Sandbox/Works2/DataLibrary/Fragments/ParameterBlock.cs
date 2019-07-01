namespace DataLibrary.Fragments
{
    public sealed class ParameterBlock : IFragment
    {
        private readonly string value;

        public ParameterBlock(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IFragmentContext context) => context.IsDynamicParameter(value);

        public void Build(IFragmentCodeBuilder builder) => builder.AddParameter(value);
    }
}
