namespace DataLibrary.Attributes
{
    public sealed class QueryAttribute : LoaderMethodAttribute
    {
        public QueryAttribute()
            : base(MethodType.Query)
        {
        }
    }
}
