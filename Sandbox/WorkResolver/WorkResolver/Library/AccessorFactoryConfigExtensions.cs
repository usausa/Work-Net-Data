namespace WorkResolver.Library
{
    public static class AccessorFactoryConfigExtensions
    {
        public static AccessorFactory ToAccessorFactory(this AccessorFactoryConfig config)
        {
            return new AccessorFactory(config);
        }
    }
}
