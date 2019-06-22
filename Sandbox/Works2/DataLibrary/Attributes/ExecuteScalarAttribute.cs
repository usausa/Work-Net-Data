namespace DataLibrary.Attributes
{
    public sealed class ExecuteScalarAttribute : LoaderMethodAttribute
    {
        public ExecuteScalarAttribute()
            : this(string.Empty)
        {
        }

        public ExecuteScalarAttribute(string value)
            : base(MethodType.ExecuteScalar, value)
        {
        }
    }
}
