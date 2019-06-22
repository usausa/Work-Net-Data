namespace DataLibrary.Attributes
{
    public sealed class QueryFirstOrDefaultAttribute : LoaderMethodAttribute
    {
        public QueryFirstOrDefaultAttribute()
            : this(string.Empty)
        {
        }

        public QueryFirstOrDefaultAttribute(string value)
            : base(MethodType.QueryFirstOrDefault, value)
        {
        }
    }
}
