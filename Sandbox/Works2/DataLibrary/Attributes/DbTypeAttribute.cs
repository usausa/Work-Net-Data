namespace DataLibrary.Attributes
{
    using System;

    using System.Data;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class DbTypeAttribute : ParameterAttribute
    {
        private readonly DbType dbType;

        private readonly int? size;

        public DbTypeAttribute(DbType dbType)
        {
            this.dbType = dbType;
            this.size = null;
        }

        public DbTypeAttribute(DbType dbType, int size)
        {
            this.dbType = dbType;
            this.size = size;
        }

        public override Action<IDbDataParameter, object> CreateSetAction(Type type)
        {
            if (size.HasValue)
            {
                return (parameter, value) =>
                {
                    parameter.DbType = dbType;
                    parameter.Size = size.Value;
                    parameter.Value = value;
                };
            }

            return (parameter, value) =>
            {
                parameter.DbType = dbType;
                parameter.Value = value;
            };
        }
    }
}
