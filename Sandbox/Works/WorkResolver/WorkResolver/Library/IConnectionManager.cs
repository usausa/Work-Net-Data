namespace WorkResolver.Library
{
    using System;
    using System.Data;

    public interface IConnectionManager
    {
        Func<IDbConnection> GetFactory(string name);
    }
}
