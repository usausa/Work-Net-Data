namespace WorkResolver.Usage.SmartResolverUsage
{
    using System;
    using System.Data;

    using Smart.Resolver;

    using WorkResolver.External;
    using WorkResolver.Library;

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
