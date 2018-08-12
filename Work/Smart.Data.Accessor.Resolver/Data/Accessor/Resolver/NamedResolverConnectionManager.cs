namespace Smart.Data.Accessor.Resolver
{
    using System;
    using System.Data;

    using Smart.Data.Accessor.Connection;
    using Smart.Resolver;

    public sealed class NamedResolverConnectionManager : IConnectionManager
    {
        private readonly IKernel kernel;

        public NamedResolverConnectionManager(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public Func<IDbConnection> GetFactory(string name)
        {
            return kernel.Get<IConnectionFactory>(name).CreateConnection;
        }
    }
}
