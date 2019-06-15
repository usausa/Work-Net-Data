//namespace DataLibrary.Engine
//{
//    using System;
//    using System.Data;
//    using System.Data.Common;
//    using System.Runtime.CompilerServices;

//    using DataLibrary.Handlers;

//    public interface IInParameterAction
//    {
//        void Setup(DbCommand cmd, object value);
//    }

//    public sealed class InParameterAction : IInParameterAction
//    {
//        private readonly string name;

//        private readonly DbType dbType;

//        public InParameterAction(string name, DbType dbType)
//        {
//            this.name = name;
//            this.dbType = dbType;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public void Setup(DbCommand cmd, object value)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            if (value is null)
//            {
//                parameter.Value = DBNull.Value;
//            }
//            else
//            {
//                parameter.DbType = dbType;
//                parameter.Value = value;
//            }
//        }
//    }

//    public sealed class InParameterActionWithSize : IInParameterAction
//    {
//        private readonly string name;

//        private readonly DbType dbType;

//        private readonly int size;

//        public InParameterActionWithSize(string name, DbType dbType, int size)
//        {
//            this.name = name;
//            this.dbType = dbType;
//            this.size = size;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public void Setup(DbCommand cmd, object value)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            if (value is null)
//            {
//                parameter.Value = DBNull.Value;
//            }
//            else
//            {
//                parameter.DbType = dbType;
//                parameter.Size = size;
//                parameter.Value = value;
//            }
//        }
//    }

//    public sealed class InParameterActionByHandler<T> : IInParameterAction
//        where T : ITypeHandler
//    {
//        private readonly string name;

//        private readonly T handler;

//        public InParameterActionByHandler(string name, T handler)
//        {
//            this.name = name;
//            this.handler = handler;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public void Setup(DbCommand cmd, object value)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            if (value is null)
//            {
//                parameter.Value = DBNull.Value;
//            }
//            else
//            {
//                handler.SetValue(parameter, value);
//            }
//        }
//    }

//    public interface IInOutParameterAction
//    {
//        DbParameter Setup(DbCommand cmd, object value);
//    }

//    public sealed class InOutParameterAction : IInOutParameterAction
//    {
//        private readonly string name;

//        private readonly DbType dbType;

//        public InOutParameterAction(string name, DbType dbType)
//        {
//            this.name = name;
//            this.dbType = dbType;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public DbParameter Setup(DbCommand cmd, object value)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            parameter.Direction = ParameterDirection.InputOutput;
//            if (value is null)
//            {
//                parameter.Value = DBNull.Value;
//            }
//            else
//            {
//                parameter.DbType = dbType;
//                parameter.Value = value;
//            }

//            return parameter;
//        }
//    }

//    public sealed class InOutParameterActionWithSize : IInOutParameterAction
//    {
//        private readonly string name;

//        private readonly DbType dbType;

//        private readonly int size;

//        public InOutParameterActionWithSize(string name, DbType dbType, int size)
//        {
//            this.name = name;
//            this.dbType = dbType;
//            this.size = size;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public DbParameter Setup(DbCommand cmd, object value)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            parameter.Direction = ParameterDirection.InputOutput;
//            if (value is null)
//            {
//                parameter.Value = DBNull.Value;
//            }
//            else
//            {
//                parameter.DbType = dbType;
//                parameter.Size = size;
//                parameter.Value = value;
//            }

//            return parameter;
//        }
//    }

//    public sealed class InOutParameterActionByHandler<T> : IInOutParameterAction
//        where T : ITypeHandler
//    {
//        private readonly string name;

//        private readonly T handler;

//        public InOutParameterActionByHandler(string name, T handler)
//        {
//            this.name = name;
//            this.handler = handler;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public DbParameter Setup(DbCommand cmd, object value)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            parameter.Direction = ParameterDirection.InputOutput;
//            if (value is null)
//            {
//                parameter.Value = DBNull.Value;
//            }
//            else
//            {
//                handler.SetValue(parameter, value);
//            }

//            return parameter;
//        }
//    }

//    public interface IOutParameterAction
//    {
//        DbParameter Setup(DbCommand cmd);
//    }

//    public sealed class OutParameterAction : IOutParameterAction
//    {
//        private readonly string name;

//        public OutParameterAction(string name)
//        {
//            this.name = name;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public DbParameter Setup(DbCommand cmd)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            parameter.Direction = ParameterDirection.Output;
//            return parameter;
//        }
//    }

//    public sealed class ReturnParameterAction : IOutParameterAction
//    {
//        private readonly string name;

//        public ReturnParameterAction(string name)
//        {
//            this.name = name;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public DbParameter Setup(DbCommand cmd)
//        {
//            var parameter = cmd.CreateParameter();
//            parameter.ParameterName = name;
//            parameter.Direction = ParameterDirection.ReturnValue;
//            return parameter;
//        }
//    }
//}
