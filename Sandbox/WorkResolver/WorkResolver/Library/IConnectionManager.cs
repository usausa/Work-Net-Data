namespace WorkResolver.Library
{
    using System;

    public interface IConnectionManager
    {
        Func<IConnection> GetFactory(string name);
    }
}
