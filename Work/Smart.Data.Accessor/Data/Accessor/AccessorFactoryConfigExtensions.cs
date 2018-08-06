namespace Smart.Data.Accessor
{
    public static class AccessorFactoryConfigExtensions
    {
        public static AccessorFactory ToAccessorFactory(this AccessorFactoryConfig config)
        {
            return new AccessorFactory(config);
        }
    }
}
