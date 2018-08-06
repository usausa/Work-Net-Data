namespace Smart.Data.Accessor.Connection
{
    using System;
    using System.Data;

    public class ConnectionFactoryConnectionManager : IConnectionManager
    {
        private readonly IConnectionFactory factory;

        public ConnectionFactoryConnectionManager(IConnectionFactory factory)
        {
            this.factory = factory;
        }

        public Func<IDbConnection> GetFactory(string name)
        {
            return factory.CreateConnection;
        }
    }
}
