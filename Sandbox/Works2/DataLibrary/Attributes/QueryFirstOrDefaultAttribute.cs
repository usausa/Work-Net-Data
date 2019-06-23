namespace DataLibrary.Attributes
{
    public sealed class QueryFirstOrDefaultAttribute : LoaderMethodAttribute
    {
        public QueryFirstOrDefaultAttribute()
            : base(MethodType.QueryFirstOrDefault)
        {
        }
    }
}
