namespace Smart.Data.Accessor.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Smart.Data.Accessor.Attributes;

    public sealed partial class ExecuteEngine
    {
        //--------------------------------------------------------------------------------
        // In
        //--------------------------------------------------------------------------------

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
                else if (handler != null)
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
                parameter.ParameterName = name;
            }
        }

        public InParameterSetup<T> CreateInParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new InParameterSetup<T>(null, attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new InParameterSetup<T>(handler.SetValue, DbType.Object, null);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new InParameterSetup<T>(null, dbType, null);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // In/Out
        //--------------------------------------------------------------------------------

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

            public DbParameter Setup(DbCommand cmd, string name, T value)
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (value == null)
                {
                    parameter.Value = DBNull.Value;
                }
                else if (handler != null)
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
                parameter.ParameterName = name;
                parameter.Direction = ParameterDirection.InputOutput;
                return parameter;
            }
        }

        public InOutParameterSetup<T> CreateInOutParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new InOutParameterSetup<T>(null, attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new InOutParameterSetup<T>(handler.SetValue, DbType.Object, null);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new InOutParameterSetup<T>(null, dbType, null);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // Out
        //--------------------------------------------------------------------------------

        public sealed class OutParameterSetup
        {
            private readonly DbType dbType;

            private readonly int? size;

            public OutParameterSetup(DbType dbType, int? size)
            {
                this.dbType = dbType;
                this.size = size;
            }

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

        public OutParameterSetup CreateOutParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new OutParameterSetup(attribute.DbType, attribute.Size);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new OutParameterSetup(dbType, null);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // Return
        //--------------------------------------------------------------------------------

        public sealed class ReturnParameterSetup
        {
            public static ReturnParameterSetup Instance { get; } = new ReturnParameterSetup();

            private ReturnParameterSetup()
            {
            }

            public DbParameter Setup(DbCommand cmd)
            {
                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                parameter.Direction = ParameterDirection.ReturnValue;
                return parameter;
            }
        }

        public ReturnParameterSetup CreateReturnParameterSetup() => ReturnParameterSetup.Instance;

        //--------------------------------------------------------------------------------
        // Array
        //--------------------------------------------------------------------------------

        public sealed class ArrayParameterSetup<T>
        {
            private readonly ExecuteEngine engine;

            private readonly Action<DbParameter, object> handler;

            private readonly DbType dbType;

            private readonly int? size;

            public ArrayParameterSetup(ExecuteEngine engine, Action<DbParameter, object> handler, DbType dbType, int? size)
            {
                this.engine = engine;
                this.handler = handler;
                this.dbType = dbType;
                this.size = size;
            }

            public void AppendSql(StringBuilder sql, string name, T[] values)
            {
                sql.Append("(");

                if ((values is null) || (values.Length == 0))
                {
                    sql.Append(engine.emptyDialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        sql.Append(name);
                        sql.Append(engine.GetParameterSubName(i));
                        sql.Append(", ");
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            }

            public void Setup(DbCommand cmd, string name, T[] values)
            {
                if (values == null)
                {
                    return;
                }

                for (var i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    if (value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else if (handler != null)
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
                    parameter.ParameterName = name + engine.GetParameterSubName(i);
                }
            }
        }

        public ArrayParameterSetup<T> CreateArrayParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new ArrayParameterSetup<T>(this, null, attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new ArrayParameterSetup<T>(this, handler.SetValue, DbType.Object, null);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new ArrayParameterSetup<T>(this, null, dbType, null);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // IList
        //--------------------------------------------------------------------------------

        public sealed class ListParameterSetup<T>
        {
            private readonly ExecuteEngine engine;

            private readonly Action<DbParameter, object> handler;

            private readonly DbType dbType;

            private readonly int? size;

            public ListParameterSetup(ExecuteEngine engine, Action<DbParameter, object> handler, DbType dbType, int? size)
            {
                this.engine = engine;
                this.handler = handler;
                this.dbType = dbType;
                this.size = size;
            }

            public void AppendSql(StringBuilder sql, string name, IList<T> values)
            {
                sql.Append("(");

                if ((values is null) || (values.Count == 0))
                {
                    sql.Append(engine.emptyDialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Count; i++)
                    {
                        sql.Append(name);
                        sql.Append(engine.GetParameterSubName(i));
                        sql.Append(", ");
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");
            }

            public void Setup(DbCommand cmd, string name, IList<T> values)
            {
                if (values is null)
                {
                    return;
                }

                for (var i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    if (value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else if (handler != null)
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
                    parameter.ParameterName = name + engine.GetParameterSubName(i);
                }
            }
        }

        public ListParameterSetup<T> CreateListParameterSetup<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new ListParameterSetup<T>(this, null, attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new ListParameterSetup<T>(this, handler.SetValue, DbType.Object, null);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new ListParameterSetup<T>(this, null, dbType, null);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // Dynamic
        //--------------------------------------------------------------------------------

        //public sealed class DynamicParameterSetup
        //{
        //}

        public Action<DbCommand, StringBuilder, string, object> CreateDynamicParameterSetup()
        {
            var holder = new DynamicParameterSetupHolder { Setup = DynamicParameterSetup.Empty };
            return (cmd, sql, name, value) =>
            {
                if (value == null)
                {
                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    parameter.Value = DBNull.Value;
                    parameter.ParameterName = name;
                }
                else
                {
                    var setup = holder.Setup;
                    if (value.GetType() != setup.Type)
                    {
                        var type = value.GetType();
                        if (!dynamicSetupCache.TryGetValue(type, out setup))
                        {
                            setup = dynamicSetupCache.AddIfNotExist(type, CreateDynamicParameterSetup);
                        }

                        holder.Setup = setup;
                    }

                    if (setup.SqlSetup is null)
                    {
                        sql.Append(name);
                    }
                    else
                    {
                        setup.SqlSetup?.Invoke(name, sql, value);
                    }
                    setup.ParameterSetup(cmd, name, value);
                }
            };
        }

        private DynamicParameterSetup CreateDynamicParameterSetup(Type type)
        {
            //if (TypeHelper.IsArrayParameter(type))
            //{
            //    var createSqlMethod = GetType().GetMethod("CreateArraySqlSetup", BindingFlags.NonPublic).MakeGenericMethod(type);
            //    var sqlSetup = createSqlMethod.Invoke(this, null);

            //    // ITypeHandler
            //    if (LookupTypeHandler(type, out var handler))
            //    {
            //        return CreateArrayParameterSetupByHandler<T>(handler.SetValue);
            //    }

            //    Type
            //    if (LookupDbType(type, out var dbType))
            //    {

            //        return CreateArrayParameterSetupByDbType<T>(dbType, null);
            //        return new DynamicParameterSetup(type, sqlSetup, CreateInParameterSetupByHandler<object>(handler.SetValue));
            //    }
            //}
            //else if (TypeHelper.IsListParameter(type))
            //{
            //    // TODO
            //}
            //else
            //{
            //    // [MEMO] Box if value type

            //    // ITypeHandler
            //    if (LookupTypeHandler(type, out var handler))
            //    {
            //        return new DynamicParameterSetup(type, null, CreateInParameterSetupByHandler<object>(handler.SetValue));
            //    }

            //    // Type
            //    if (LookupDbType(type, out var dbType))
            //    {
            //        return new DynamicParameterSetup(type, null, CreateInParameterSetupByDbType<object>(dbType, null));
            //    }
            //}

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        // TODO これを<T>にする？
        private sealed class DynamicParameterSetup
        {
            public static DynamicParameterSetup Empty { get; } = new DynamicParameterSetup(null, null, null);

            public Type Type { get; }

            public Action<string, StringBuilder, object> SqlSetup { get; }

            public Action<DbCommand, string, object> ParameterSetup { get; }

            public DynamicParameterSetup(Type type, Action<string, StringBuilder, object> sqlSetup, Action<DbCommand, string, object> parameterSetup)
            {
                Type = type;
                SqlSetup = sqlSetup;
                ParameterSetup = parameterSetup;
            }
        }

        private class DynamicParameterSetupHolder
        {
            public DynamicParameterSetup Setup { get; set; }
        }
    }
}