using System.Data;

namespace DataLibrary.Attributes
{
    using System;

    using Smart.Functional;

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

        public override Action<IDbDataParameter, object> CreateSetAction()
        {
            if (size.HasValue)
            {
                return (parameter, value) =>
                {
                    if (value is null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.DbType = dbType;
                        parameter.Size = size.Value;
                        parameter.Value = value;
                    }
                };
            }

            return (parameter, value) =>
            {
                if (value is null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.DbType = dbType;
                    parameter.Value = value;
                }
            };
        }
    }
}
