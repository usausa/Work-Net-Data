namespace WorkResolver.Library
{
    using System;
    using System.Data;

    public class ConnectionFactoryConnectionManager : IConnectionManager
    {
        public Func<IDbConnection> GetFactory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
