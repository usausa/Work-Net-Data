namespace DataLibrary.Providers
{
    public interface INamedDbProviderFactory
    {
        IDbProvider GetProvider(string name);
    }
}
