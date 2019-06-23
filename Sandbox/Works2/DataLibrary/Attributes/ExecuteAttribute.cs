namespace DataLibrary.Attributes
{
    public sealed class ExecuteAttribute : LoaderMethodAttribute
    {
        public ExecuteAttribute()
            : base(MethodType.Execute)
        {
        }
    }
}
