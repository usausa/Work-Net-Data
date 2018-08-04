namespace WorkResolver.Library
{
    using System;

    public class AccessorFactory : IAccessorFactory
    {
        private readonly IAccessorFactoryConfig config;

        public AccessorFactory(IAccessorFactoryConfig config)
        {
            this.config = config;
        }

        public T Create<T>()
        {
            var ifType = typeof(T);
            var implName = ifType.Namespace + "." + ifType.Name.Substring(1) + "Impl";
            var implType = ifType.Assembly.GetType(implName);
            return (T)Activator.CreateInstance(implType, config.Executor, config.ConnectionManager);
        }
    }
}
