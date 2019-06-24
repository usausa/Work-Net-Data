namespace DataLibrary.Providers
{
    public interface IDbProviderSelector
    {
        IDbProvider Select(object parameter);
    }
}
