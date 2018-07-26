using System;
namespace WorkResolver.Library
{
    using System.Data;

    public class MultipleConnectionManager : IConnectionManager
    {
        public Func<IDbConnection> GetFactory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
