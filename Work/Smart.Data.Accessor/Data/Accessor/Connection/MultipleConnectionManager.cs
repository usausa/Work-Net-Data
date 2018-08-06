namespace Smart.Data.Accessor.Connection
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class MultipleConnectionManager : IConnectionManager
    {
        private readonly IDictionary<string, Func<IDbConnection>> factories;

        public MultipleConnectionManager(IDictionary<string, Func<IDbConnection>> factories)
        {
            this.factories = factories;
        }

        public Func<IDbConnection> GetFactory(string name)
        {
            return factories[name];
        }
    }
}
