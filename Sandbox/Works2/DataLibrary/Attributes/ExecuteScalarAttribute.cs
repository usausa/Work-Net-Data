namespace DataLibrary.Attributes
{
    public sealed class ExecuteScalarAttribute : LoaderMethodAttribute
    {
        public ExecuteScalarAttribute()
            : base(MethodType.ExecuteScalar)
        {
        }
    }
}
