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

        public Action<DbCommand, string, object> CreateInParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateInParameterSetupByAction(type, attribute.CreateSetAction(type));
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateInParameterSetupByAction(type, handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateInParameterSetupByDbType(type, dbType);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Action<DbCommand, string, object> CreateInParameterSetupByAction(Type type, Action<IDbDataParameter, object> action)
        {
            if (type.IsValueType)
            {
                return (cmd, name, value) =>
                {
                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    action(parameter, value);
                    parameter.ParameterName = name;
                };
            }

            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
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

        private static Action<DbCommand, string, object> CreateInParameterSetupByDbType(Type type, DbType dbType)
        {
            if (type.IsValueType)
            {
                return (cmd, name, value) =>
                {
                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    parameter.DbType = dbType;
                    parameter.Value = value;
                    parameter.ParameterName = name;
                };
            }

            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
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

        public Func<DbCommand, string, object, DbParameter> CreateInOutParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateInOutParameterSetupByAction(type, attribute.CreateSetAction(type));
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateInOutParameterSetupByAction(type, handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateInOutParameterSetupByDbType(type, dbType);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Func<DbCommand, string, object, DbParameter> CreateInOutParameterSetupByAction(Type type, Action<IDbDataParameter, object> action)
        {
            if (type.IsValueType)
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

            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
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

        private static Func<DbCommand, string, object, DbParameter> CreateInOutParameterSetupByDbType(Type type, DbType dbType)
        {
            if (type.IsValueType)
            {
                return (cmd, name, value) =>
                {
                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    parameter.DbType = dbType;
                    parameter.Value = value;
                    parameter.ParameterName = name;
                    parameter.Direction = ParameterDirection.InputOutput;
                    return parameter;
                };
            }

            return (cmd, name, value) =>
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
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

        public Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateArrayParameterSetupByAction<T>(attribute.CreateSetAction(type));
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateArrayParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateArrayParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        public Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
        {
            if (typeof(T).IsValueType)
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
            if (typeof(T).IsValueType)
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
                            parameter.DbType = dbType;
                            parameter.Value = value;
                            parameter.ParameterName = name;
                        }

                        sql.Length -= 2;
                    }

                    sql.Append(") ");
                };
            }

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

        public Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateListParameterSetupByAction<T>(attribute.CreateSetAction(type));
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateListParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateListParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
        {
            if (typeof(T).IsValueType)
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
            if (typeof(T).IsValueType)
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
                            parameter.DbType = dbType;
                            parameter.Value = value;
                            parameter.ParameterName = name;
                        }

                        sql.Length -= 2;
                    }

                    sql.Append(") ");
                };
            }

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

        public Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterAttribute
            var attribute = provider?.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return CreateEnumerableParameterSetupByAction<T>(attribute.CreateSetAction(type));
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return CreateEnumerableParameterSetupByAction<T>(handler.SetValue);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return CreateEnumerableParameterSetupByDbType<T>(dbType);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetupByAction<T>(Action<IDbDataParameter, object> action)
        {
            if (typeof(T).IsValueType)
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
            if (typeof(T).IsValueType)
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
                        parameter.DbType = dbType;
                        parameter.Value = value;
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
