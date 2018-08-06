namespace Smart.Data.Accessor.Connection
{
    using System;
    using System.Data;

    public sealed class SingleConnectionManager : IConnectionManager
    {
        private readonly Func<IDbConnection> func;

        public SingleConnectionManager(Func<IDbConnection> func)
        {
            this.func = func;
        }

        public Func<IDbConnection> GetFactory(string name)
        {
            return func;
        }
    }
}
