namespace DataLibrary.Attributes
{
    public sealed class QueryAttribute : LoaderMethodAttribute
    {
        public QueryAttribute()
            : this(string.Empty)
        {
        }

        public QueryAttribute(string value)
            : base(MethodType.Query, value)
        {
        }
    }
}
