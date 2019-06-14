namespace DataLibrary.Engine
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.CompilerServices;

    using DataLibrary.Handlers;

    public interface IInParameterBuilder
    {
        void Build(DbCommand cmd, object value);
    }

    public sealed class InParameterBuilder : IInParameterBuilder
    {
        private readonly string name;

        private readonly DbType dbType;

        public InParameterBuilder(string name, DbType dbType)
        {
            this.name = name;
            this.dbType = dbType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build(DbCommand cmd, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.DbType = dbType;
                parameter.Value = value;
            }
        }
    }

    public sealed class InParameterBuilderWithSize : IInParameterBuilder
    {
        private readonly string name;

        private readonly DbType dbType;

        private readonly int size;

        public InParameterBuilderWithSize(string name, DbType dbType, int size)
        {
            this.name = name;
            this.dbType = dbType;
            this.size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build(DbCommand cmd, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.DbType = dbType;
                parameter.Size = size;
                parameter.Value = value;
            }
        }
    }

    public sealed class InParameterBuilderByHandler<T> : IInParameterBuilder
        where T : ITypeHandler
    {
        private readonly string name;

        private readonly T handler;

        public InParameterBuilderByHandler(string name, T handler)
        {
            this.name = name;
            this.handler = handler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Build(DbCommand cmd, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                handler.SetValue(parameter, value);
            }
        }
    }

    public interface IInOutParameterBuilder
    {
        DbParameter Build(DbCommand cmd, object value);
    }

    public sealed class InOutParameterBuilder : IInOutParameterBuilder
    {
        private readonly string name;

        private readonly DbType dbType;

        public InOutParameterBuilder(string name, DbType dbType)
        {
            this.name = name;
            this.dbType = dbType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Build(DbCommand cmd, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.InputOutput;
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.DbType = dbType;
                parameter.Value = value;
            }

            return parameter;
        }
    }

    public sealed class InOutParameterBuilderWithSize : IInOutParameterBuilder
    {
        private readonly string name;

        private readonly DbType dbType;

        private readonly int size;

        public InOutParameterBuilderWithSize(string name, DbType dbType, int size)
        {
            this.name = name;
            this.dbType = dbType;
            this.size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Build(DbCommand cmd, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.InputOutput;
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.DbType = dbType;
                parameter.Size = size;
                parameter.Value = value;
            }

            return parameter;
        }
    }

    public sealed class InOutParameterBuilderByHandler<T> : IInOutParameterBuilder
        where T : ITypeHandler
    {
        private readonly string name;

        private readonly T handler;

        public InOutParameterBuilderByHandler(string name, T handler)
        {
            this.name = name;
            this.handler = handler;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Build(DbCommand cmd, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.InputOutput;
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                handler.SetValue(parameter, value);
            }

            return parameter;
        }
    }

    public interface IOutParameterBuilder
    {
        DbParameter Build(DbCommand cmd);
    }

    public sealed class OutParameterBuilder : IOutParameterBuilder
    {
        private readonly string name;

        public OutParameterBuilder(string name)
        {
            this.name = name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Build(DbCommand cmd)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }
    }

    public sealed class ReturnParameterBuilder : IOutParameterBuilder
    {
        private readonly string name;

        public ReturnParameterBuilder(string name)
        {
            this.name = name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbParameter Build(DbCommand cmd)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.ReturnValue;
            return parameter;
        }
    }
}
