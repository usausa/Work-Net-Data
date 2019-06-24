namespace DataLibrary.Providers
{
    using System.Collections.Generic;

    public sealed class NamedDbProviderSelector : IDbProviderSelector
    {
        private readonly Dictionary<string, IDbProvider> providers = new Dictionary<string, IDbProvider>();

        public void AddProvider(string name, IDbProvider provider)
        {
            providers[name] = provider;
        }

        public IDbProvider Select(object parameter)
        {
            var key = (string)parameter;
            if (providers.TryGetValue(key, out var provider))
            {
                return provider;
            }

            throw new AccessorException($"Provider is not found. name=[{key}]");
        }
    }
}
