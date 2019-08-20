namespace Smart.Data.Accessor.Engine
{
    using System;
    using System.Data;
    using System.Data.Common;

    public sealed class InParameterSetup<T>
    {
        private readonly Action<DbParameter, object> handler;

        private readonly DbType dbType;

        private readonly int? size;

        public InParameterSetup(Action<DbParameter, object> handler, DbType dbType, int? size)
        {
            this.handler = handler;
            this.dbType = dbType;
            this.size = size;
        }

        public void Setup(DbCommand cmd, string name, T value)
        {
            var parameter = cmd.CreateParameter();
            cmd.Parameters.Add(parameter);
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                if (handler != null)
                {
                    handler(parameter, value);
                }
                else
                {
                    parameter.Value = value;
                    parameter.DbType = dbType;
                    if (size.HasValue)
                    {
                        parameter.Size = size.Value;
                    }
                }
            }
            parameter.ParameterName = name;
        }
    }

    public sealed class InOutParameterSetup
    {
    }

    public sealed class OutParameterSetup
    {
    }

    public sealed class ArrayParameterSetup
    {
    }

    public sealed class ListParameterSetup
    {
    }

    public sealed class DynamicParameterSetup
    {
    }
}
