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

    using Smart;

    public sealed partial class ExecuteEngine
    {
        //--------------------------------------------------------------------------------
        // In
        //--------------------------------------------------------------------------------

        public Action<DbCommand, string, T> CreateInParameterSetup<T>(ICustomAttributeProvider provider)
        {
            // TODO extension ?
            var type = typeof(T).IsNullableType() ? Nullable.GetUnderlyingType(typeof(T)) : typeof(T);

            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateInParameterSetupByAction<T>(attribute.CreateSetAction());
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return CreateInParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return CreateInParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Action<DbCommand, string, T> CreateInParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (value == null)  // MEMO 最適化
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

        private static Action<DbCommand, string, T> CreateInParameterSetupByDbType<T>(DbType dbType)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (value == null)  // MEMO 最適化
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

        public Func<DbCommand, string, T, DbParameter> CreateInOutParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T).IsNullableType() ? Nullable.GetUnderlyingType(typeof(T)) : typeof(T);

            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateInOutParameterSetupByAction<T>(attribute.CreateSetAction());
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return CreateInOutParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return CreateInOutParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Func<DbCommand, string, T, DbParameter> CreateInOutParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (value == null)  // MEMO 最適化
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

        private static Func<DbCommand, string, T, DbParameter> CreateInOutParameterSetupByDbType<T>(DbType dbType)
        {
            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (value == null)  // MEMO 最適化
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

        public Func<DbCommand, string, DbParameter> CreateOutParameterSetup(ParameterDirection direction)
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
        // Array
        //--------------------------------------------------------------------------------

        public Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetup<T>(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateArrayParameterSetupByAction<T>(attribute.CreateSetAction());
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return CreateArrayParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return CreateArrayParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        public Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
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
                        var parameterName = String.Concat(name, "_", GetParameterSubName(i));

                        sql.Append(parameterName);
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        if (value == null)  // MEMO 最適化
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            action(parameter, value);
                        }
                        parameter.ParameterName = name;
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }

        private Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetupByDbType<T>(DbType dbType)
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
                        var parameterName = String.Concat(name, "_", GetParameterSubName(i));

                        sql.Append(parameterName);
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        if (value == null)  // MEMO 最適化
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            parameter.DbType = dbType;
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

        public Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetup<T>(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateListParameterSetupByAction<T>(attribute.CreateSetAction());
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return CreateListParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return CreateListParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
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
                        var parameterName = String.Concat(name, "_", GetParameterSubName(i));

                        sql.Append(parameterName);
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        if (value == null)  // MEMO 最適化
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            action(parameter, value);
                        }
                        parameter.ParameterName = name;
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            };
        }

        private Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetupByDbType<T>(DbType dbType)
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
                        var parameterName = String.Concat(name, "_", GetParameterSubName(i));

                        sql.Append(parameterName);
                        sql.Append(", ");

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        if (value == null)  // MEMO 最適化
                        {
                            parameter.Value = DBNull.Value;
                        }
                        else
                        {
                            parameter.DbType = dbType;
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

        public Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetup<T>(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateEnumerableParameterSetupByAction<T>(attribute.CreateSetAction());
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return CreateEnumerableParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return CreateEnumerableParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                var i = 0;
                foreach (var value in values)
                {
                    var parameterName = String.Concat(name, "_", GetParameterSubName(i));

                    sql.Append(parameterName);
                    sql.Append(", ");

                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    if (value == null)  // MEMO 最適化
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        action(parameter, value);
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

        private Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetupByDbType<T>(DbType dbType)
        {
            return (cmd, name, sql, values) =>
            {
                sql.Append("(");

                var i = 0;
                foreach (var value in values)
                {
                    var parameterName = String.Concat(name, "_", GetParameterSubName(i));

                    sql.Append(parameterName);
                    sql.Append(", ");

                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    if (value == null)  // MEMO 最適化
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.DbType = dbType;
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
