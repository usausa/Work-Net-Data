using System;
namespace WorkResolver.Library
{
    using System.Data;

    public class CallbackConnectionManager : IConnectionManager
    {
        public Func<IDbConnection> GetFactory(string name)
        {
            throw new NotImplementedException();
        }
    }
}
