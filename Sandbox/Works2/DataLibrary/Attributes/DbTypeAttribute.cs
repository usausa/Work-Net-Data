namespace DataLibrary.Attributes
{
    using System;

    using System.Data;

    public sealed class DbTypeBuilderAttribute : ParameterBuilderAttribute
    {
        private readonly DbType dbType;

        private readonly int? size;

        public DbTypeBuilderAttribute(DbType dbType)
        {
            this.dbType = dbType;
            size = null;
        }

        public DbTypeBuilderAttribute(DbType dbType, int size)
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
