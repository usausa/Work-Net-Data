namespace DataLibrary.Fragments
{
    public sealed class SqlFragment : IFragment
    {
        private readonly string value;

        public SqlFragment(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IFragmentContext context) => false;

        public void Build(IFragmentCodeBuilder builder) => builder.AddSql(value);
    }
}
