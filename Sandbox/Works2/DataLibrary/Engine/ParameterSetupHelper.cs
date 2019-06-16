namespace DataLibrary.Engine
{
    using System;
    using System.Data;
    using System.Data.Common;

    public static class ParameterSetupHelper
    {
        public static Action<DbCommand, object> CreateInParameterSetupByAction(string name, Action<IDbDataParameter, object> action)
        {
            return (cmd, value) =>
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

        public static Action<DbCommand, object> CreateInParameterSetupByDbType(string name, DbType dbType)
        {
            return (cmd, value) =>
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

        public static Func<DbCommand, object, DbParameter> CreateInOutParameterSetupByAction(string name, Action<IDbDataParameter, object> action)
        {
            return (cmd, value) =>
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

        public static Func<DbCommand, object, DbParameter> CreateInOutParameterSetupByDbType(string name, DbType dbType)
        {
            return (cmd, value) =>
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

        public static Func<DbCommand, DbParameter> CreateOutParameterSetup(string name, ParameterDirection direction)
        {
            return cmd =>
            {
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = name;
                parameter.Direction = direction;
                return parameter;
            };
        }
    }
}
