namespace DataLibrary.Attributes
{
    public sealed class ExecuteScalarReader : LoaderMethodAttribute
    {
        public ExecuteScalarReader()
            : this(string.Empty)
        {
        }

        public ExecuteScalarReader(string value)
            : base(MethodType.ExecuteReader, value)
        {
        }
    }
}
