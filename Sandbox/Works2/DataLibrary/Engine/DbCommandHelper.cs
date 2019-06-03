namespace DataLibrary.Engine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Data;
    using System.Data.Common;

    using DataLibrary.Handlers;

    public static class DbCommandHelper
    {
        //--------------------------------------------------------------------------------
        // Without direction
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddParameter(DbCommand cmd, string name, DbType dbType, object value)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterAndReturn(DbCommand cmd, string name, DbType dbType, object value)
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
            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddParameter(DbCommand cmd, string name, DbType dbType, int size, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterAndReturn(DbCommand cmd, string name, DbType dbType, int size, object value)
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
            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddParameter<THandler>(DbCommand cmd, string name, THandler handler, object value) where THandler : ITypeHandler
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterAndReturn<THandler>(DbCommand cmd, string name, THandler handler, object value) where THandler : ITypeHandler
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
            return parameter;
        }

        //--------------------------------------------------------------------------------
        // With direction
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterWithDirectionAndReturn(DbCommand cmd, string name, ParameterDirection direction, DbType dbType)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
            parameter.DbType = dbType;
            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterWithDirectionAndReturn(DbCommand cmd, string name, ParameterDirection direction, DbType dbType, int size)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
            parameter.DbType = dbType;
            parameter.Size = size;
            return parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddParameterWithDirection(DbCommand cmd, string name, ParameterDirection direction, DbType dbType, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterWithDirectionAndReturn(DbCommand cmd, string name, ParameterDirection direction, DbType dbType, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
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

       [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddParameterWithDirection(DbCommand cmd, string name, ParameterDirection direction, DbType dbType, int size, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterWithDirectionAndReturn(DbCommand cmd, string name, ParameterDirection direction, DbType dbType, int size, object value)
        {
            // [MEMO] Full version
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddParameterWithDirection<THandler>(DbCommand cmd, string name, ParameterDirection direction, THandler handler, object value) where THandler : ITypeHandler
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
            if (value is null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                handler.SetValue(parameter, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DbParameter AddParameterWithDirectionAndReturn<THandler>(DbCommand cmd, string name, ParameterDirection direction, THandler handler, object value) where THandler : ITypeHandler
        {
            // [MEMO] Full version
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Direction = direction;
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
}
