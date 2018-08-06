namespace Smart.Data.Accessor
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
            return (T)Create(typeof(T));
        }

        public object Create(Type type)
        {
            // TODO Dao attribute, Builder attribute, no namspace
            var implName = type.Namespace + "." + type.Name.Substring(1) + "Impl";
            var implType = type.Assembly.GetType(implName);
            return Activator.CreateInstance(implType, config.Executor, config.ConnectionManager);
        }
    }
}
