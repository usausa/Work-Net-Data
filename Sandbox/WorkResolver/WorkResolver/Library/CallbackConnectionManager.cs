namespace WorkResolver.Library
{
    using System;
    using System.Data;

    public class CallbackConnectionManager : IConnectionManager
    {
        private readonly Func<string, Func<IDbConnection>> callback;

        public CallbackConnectionManager(Func<string, Func<IDbConnection>> callback)
        {
            this.callback = callback;
        }

        public Func<IDbConnection> GetFactory(string name)
        {
            return callback(name);
        }
    }
}
