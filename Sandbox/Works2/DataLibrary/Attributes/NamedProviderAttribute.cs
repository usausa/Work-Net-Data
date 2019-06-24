namespace DataLibrary.Attributes
{
    using DataLibrary.Providers;

    public sealed class NamedProviderAttribute : ProviderAttribute
    {
        public NamedProviderAttribute(string name)
            : base(typeof(NamedDbProviderSelector), name)
        {
        }
    }
}
