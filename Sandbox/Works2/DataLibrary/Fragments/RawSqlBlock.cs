namespace DataLibrary.Fragments
{
    public sealed class RawSqlFragment : IFragment
    {
        private readonly string value;

        public RawSqlFragment(string value)
        {
            this.value = value;
        }

        public bool IsDynamic(IFragmentContext context) => true;

        public void Build(IFragmentCodeBuilder builder) => builder.AddRawSql(value);
    }
}
