using System;
namespace WorkResolver.Library
{
    using System.Data;

    public class SingleConnectionManager : IConnectionManager
    {
        public Func<IDbConnection> GetFactory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
