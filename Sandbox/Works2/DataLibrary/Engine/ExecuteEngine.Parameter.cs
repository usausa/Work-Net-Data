namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using DataLibrary.Attributes;

    public sealed partial class ExecuteEngine
    {
        //--------------------------------------------------------------------------------
        // In
        //--------------------------------------------------------------------------------

        public Action<DbCommand, string, T> CreateInParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateInParameterSetupByDbType<T>(attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateInParameterSetupByHandler<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateInParameterSetupByDbType<T>(dbType, 0);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Action<DbCommand, string, T> CreateInParameterSetupByHandler<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                action(parameter, value);
                parameter.ParameterName = name;
            };
        }

        private static Action<DbCommand, string, T> CreateInParameterSetupByDbType<T>(DbType dbType, int size)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.DbType = dbType;
                    parameter.Size = size;
                    parameter.Value = value;
                }
                parameter.ParameterName = name;
            };
        }

        //--------------------------------------------------------------------------------
        // In/Out
        //--------------------------------------------------------------------------------

        public Func<DbCommand, string, T, DbParameter> CreateInOutParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateInOutParameterSetupByDbType<T>(attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateInOutParameterSetupByHandler<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateInOutParameterSetupByDbType<T>(dbType, 0);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Func<DbCommand, string, T, DbParameter> CreateInOutParameterSetupByHandler<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                action(parameter, value);
                parameter.ParameterName = name;
                parameter.Direction = ParameterDirection.InputOutput;
                return parameter;
            };
        }

        private static Func<DbCommand, string, T, DbParameter> CreateInOutParameterSetupByDbType<T>(DbType dbType, int size)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.DbType = dbType;
                    parameter.Size = size;
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

        public Func<DbCommand, string, DbParameter> CreateOutParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateOutParameterSetupByDbType(attribute.DbType, attribute.Size);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateOutParameterSetupByDbType(dbType, 0);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Func<DbCommand, string, DbParameter> CreateOutParameterSetupByDbType(DbType dbType, int size)
        {
            return (cmd, name) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                parameter.DbType = dbType;
                parameter.Size = size;
                parameter.ParameterName = name;
                parameter.Direction = ParameterDirection.Output;
                return parameter;
            };
        }

        //--------------------------------------------------------------------------------
        // Return
        //--------------------------------------------------------------------------------

        public Func<DbCommand, string, DbParameter> CreateReturnParameterSetup()
        {
            return (cmd, name) =>
            {
                var parameter = cmd.CreateParameter();
                parameter.Direction = ParameterDirection.ReturnValue;
                return parameter;
            };
        }

        //--------------------------------------------------------------------------------
        // Array
        //--------------------------------------------------------------------------------

        public Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateArrayParameterSetupByDbType<T>(attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateArrayParameterSetupByHandler<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateArrayParameterSetupByDbType<T>(dbType, 0);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetupByHandler<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                if (values.Length == 0)
                {
                    sql.Append(emptyDialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        var value = values[i];
                        sql.Append(name);
                        sql.Append("_");
                        sql.Append(GetParameterSubName(i));
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        action(parameter, value);
                        parameter.ParameterName = name;
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }

        private Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetupByDbType<T>(DbType dbType, int size)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                if (values.Length == 0)
                {
                    sql.Append(emptyDialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        var value = values[i];
                        sql.Append(name);
                        sql.Append("_");
                        sql.Append(GetParameterSubName(i));
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        if (value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            parameter.DbType = dbType;
                            parameter.Size = size;
                            parameter.Value = value;
                        }
                        parameter.ParameterName = name;
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }

        //--------------------------------------------------------------------------------
        // IList
        //--------------------------------------------------------------------------------

        public Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateListParameterSetupByDbType<T>(attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateListParameterSetupByHandler<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateListParameterSetupByDbType<T>(dbType, 0);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetupByHandler<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                if (values.Count == 0)
                {
                    sql.Append(emptyDialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Count; i++)
                    {
                        var value = values[i];
                        sql.Append(name);
                        sql.Append("_");
                        sql.Append(GetParameterSubName(i));
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        action(parameter, value);
                        parameter.ParameterName = name;
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }

        private Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetupByDbType<T>(DbType dbType, int size)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                if (values.Count == 0)
                {
                    sql.Append(emptyDialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Count; i++)
                    {
                        var value = values[i];
                        sql.Append(name);
                        sql.Append("_");
                        sql.Append(GetParameterSubName(i));
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        if (value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            parameter.DbType = dbType;
                            parameter.Size = size;
                            parameter.Value = value;
                        }
                        parameter.ParameterName = name;
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }

        //--------------------------------------------------------------------------------
        // IEnumerable
        //--------------------------------------------------------------------------------

        public Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateEnumerableParameterSetupByDbType<T>(attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateEnumerableParameterSetupByHandler<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateEnumerableParameterSetupByDbType<T>(dbType, 0);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetupByHandler<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                var i = 0;
                foreach (var value in values)
                {
                    sql.Append(name);
                    sql.Append("_");
                    sql.Append(GetParameterSubName(i));
                    sql.Append(", ");

                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    action(parameter, value);
                    parameter.ParameterName = name;

                    i++;
                }

                if (i == 0)
                {
                    sql.Append(emptyDialect.GetSql());
                }
                else
                {
                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }

        private Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetupByDbType<T>(DbType dbType, int size)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                var i = 0;
                foreach (var value in values)
                {
                    sql.Append(name);
                    sql.Append("_");
                    sql.Append(GetParameterSubName(i));
                    sql.Append(", ");

                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    if (value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.DbType = dbType;
                        parameter.Size = size;
                        parameter.Value = value;
                    }
                    parameter.ParameterName = name;

                    i++;
                }

                if (i == 0)
                {
                    sql.Append(emptyDialect.GetSql());
                }
                else
                {
                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }
    }
}
