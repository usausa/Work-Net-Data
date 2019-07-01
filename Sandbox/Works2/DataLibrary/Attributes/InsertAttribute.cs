namespace DataLibrary.Attributes
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DataLibrary.Fragments;
    using DataLibrary.Loader;

    public sealed class InsertAttribute : MethodAttribute
    {
        private readonly string table;

        public InsertAttribute(string table)
            : base(CommandType.Text, MethodType.Execute)
        {
            this.table = table;
        }

        public override IReadOnlyList<IFragment> GetFragments(ISqlLoader loader, MethodInfo mi)
        {
            // TODO object(single and not simple) or parameters!
            var fragments = new List<IFragment>();

            fragments.Add(new SqlFragment($"INSERT INTO {table} ("));
            // TODO
            fragments.Add(new SqlFragment(") VALUES ("));
            // TODO
            fragments.Add(new SqlFragment(")"));

            return fragments;
        }
    }
}
