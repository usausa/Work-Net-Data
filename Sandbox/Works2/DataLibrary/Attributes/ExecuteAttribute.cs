namespace DataLibrary.Attributes
{
    public sealed class ExecuteAttribute : LoaderMethodAttribute
    {
        public ExecuteAttribute()
            : this(string.Empty)
        {
        }

        public ExecuteAttribute(string value)
            : base(MethodType.Execute, value)
        {
        }
    }
}
