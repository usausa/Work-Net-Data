namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Text;

    using DataLibrary.Dialect;

    public static class ParameterSetupHelper
    {
        //--------------------------------------------------------------------------------
        // In
        //--------------------------------------------------------------------------------

        public static Action<DbCommand, string, object> CreateInParameterSetupByAction(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                if (value is null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    action(parameter, value);
                }
                parameter.ParameterName = name;
            };
        }

        public static Action<DbCommand, string, object> CreateInParameterSetupByDbType(DbType dbType)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                if (value is null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.DbType = dbType;
                    parameter.Value = value;
                }
                parameter.ParameterName = name;
            };
        }

        //--------------------------------------------------------------------------------
        // In/Out
        //--------------------------------------------------------------------------------

        public static Func<DbCommand, string, object, DbParameter> CreateInOutParameterSetupByAction(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                if (value is null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    action(parameter, value);
                }
                parameter.ParameterName = name;
                parameter.Direction = ParameterDirection.InputOutput;
                return parameter;
            };
        }

        public static Func<DbCommand, string, object, DbParameter> CreateInOutParameterSetupByDbType(DbType dbType)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                if (value is null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.DbType = dbType;
                    parameter.Value = value;
                }
                parameter.ParameterName = name;
                parameter.Direction = ParameterDirection.InputOutput;
                return parameter;
            };
        }

        //--------------------------------------------------------------------------------
        // Out
        //--------------------------------------------------------------------------------

        public static Func<DbCommand, string, DbParameter> CreateOutParameterSetup(ParameterDirection direction)
        {
            return (cmd, name) =>
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = name;
                parameter.Direction = direction;
                return parameter;
            };
        }

        //--------------------------------------------------------------------------------
        // IN
        //--------------------------------------------------------------------------------

        public static Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetupByAction<T>(Action<IDbDataParameter, object> action, IEmptyDialect dialect)
        {
            return (cmd, nameof, sql, values) =>
            {
                sql.Append("(");

                if (values.Length == 0)
                {
                    sql.Append(dialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        //var

                        // TODO

                    }

                    sql.Length = sql.Length - 2;
                }

                sql.Append(") ");
            };
        }

        public static Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetupByDbType<T>(DbType dbType, IEmptyDialect dialect)
        {
            return (cmd, nameof, sql, values) =>
            {
                // TODO
            };
        }

        public static Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetupByAction<T>(Action<IDbDataParameter, object> action, IEmptyDialect dialect)
        {
            return (cmd, nameof, sql, values) =>
            {
                // TODO
            };
        }

        public static Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetupByDbType<T>(DbType dbType, IEmptyDialect dialect)
        {
            return (cmd, nameof, sql, values) =>
            {
                // TODO
            };
        }

        public static Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetupByAction<T>(Action<IDbDataParameter, object> action, IEmptyDialect dialect)
        {
            return (cmd, nameof, sql, values) =>
            {
                // TODO
            };
        }

        public static Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetupByDbType<T>(DbType dbType, IEmptyDialect dialect)
        {
            return (cmd, nameof, sql, values) =>
            {
                // TODO
            };
        }
    }
}
