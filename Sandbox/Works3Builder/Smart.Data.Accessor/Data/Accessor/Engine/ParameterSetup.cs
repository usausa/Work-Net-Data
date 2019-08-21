namespace Smart.Data.Accessor.Engine
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    public sealed class InOutParameterSetup<T>
    {
        private readonly Action<DbParameter, object> handler;

        private readonly DbType dbType;

        private readonly int? size;

        public InOutParameterSetup(Action<DbParameter, object> handler, DbType dbType, int? size)
        {
            this.handler = handler;
            this.dbType = dbType;
            this.size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Setup(DbCommand cmd, string name, T value)
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
            parameter.Direction = ParameterDirection.InputOutput;
            return parameter;
        }
    }

    public sealed class OutParameterSetup
    {
        private readonly DbType dbType;

        private readonly int? size;

        public OutParameterSetup(DbType dbType, int? size)
        {
            this.dbType = dbType;
            this.size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Setup(DbCommand cmd, string name)
        {
            var parameter = cmd.CreateParameter();
            cmd.Parameters.Add(parameter);
            parameter.DbType = dbType;
            if (size.HasValue)
            {
                parameter.Size = size.Value;
            }
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }
    }

    public sealed class ReturnParameterSetup
    {
        public static ReturnParameterSetup Instance { get; } = new ReturnParameterSetup();

        private ReturnParameterSetup()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Setup(DbCommand cmd)
        {
            var parameter = cmd.CreateParameter();
            cmd.Parameters.Add(parameter);
            parameter.Direction = ParameterDirection.ReturnValue;
            return parameter;
        }
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
