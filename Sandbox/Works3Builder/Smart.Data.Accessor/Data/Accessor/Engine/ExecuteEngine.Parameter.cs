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
    using Smart.Data.Accessor.Generator;

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

        private sealed class DynamicParameterEntry
        {
            public static DynamicParameterEntry Empty { get; } = new DynamicParameterEntry(null, null);

            public Type Type { get; }

            public Action<DbCommand, StringBuilder, string, object> Handler { get; }

            public DynamicParameterEntry(Type type, Action<DbCommand, StringBuilder, string, object> handler)
            {
                Type = type;
                Handler = handler;
            }
        }

        public sealed class DynamicParameterSetup
        {
            private readonly ExecuteEngine engine;

            private readonly bool isMultiple;

            private DynamicParameterEntry entry = DynamicParameterEntry.Empty;

            public DynamicParameterSetup(ExecuteEngine engine, bool isMultiple)
            {
                this.engine = engine;
                this.isMultiple = isMultiple;
            }

            public void Setup(DbCommand cmd, StringBuilder sql, string name, object value)
            {
                if (value is null)
                {
                    if (isMultiple)
                    {
                        sql.Append(engine.emptyDialect.GetSql());
                    }
                    else
                    {
                        sql.Append(name);

                        var parameter = cmd.CreateParameter();
                        cmd.Parameters.Add(parameter);
                        parameter.Value = DBNull.Value;
                        parameter.ParameterName = name;
                    }
                }
                else
                {
                    var type = value.GetType();
                    if (type != entry.Type)
                    {
                        entry = engine.LookupDynamicParameterEntry(type);
                    }

                    // [MEMO] Boxed if value type
                    entry.Handler(cmd, sql, name, value);
                }
            }
        }

        private DynamicParameterEntry LookupDynamicParameterEntry(Type type)
        {
            if (!dynamicSetupCache.TryGetValue(type, out var entry))
            {
                entry = dynamicSetupCache.AddIfNotExist(type, CreateDynamicParameterEntry);
            }

            return entry;
        }

        private DynamicParameterEntry CreateDynamicParameterEntry(Type type)
        {
            MethodInfo method;
            if (TypeHelper.IsArrayParameter(type))
            {
                method = GetType()
                    .GetMethod(nameof(CreateArrayParameterSetupWrapper), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(type.GetElementType());
            }
            else if (TypeHelper.IsListParameter(type))
            {
                method = GetType()
                    .GetMethod(nameof(CreateListParameterSetupWrapper), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(TypeHelper.GetListElementType(type));
            }
            else
            {
                method = GetType()
                    .GetMethod(nameof(CreateInParameterSetupWrapper), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(type);
            }

            return (DynamicParameterEntry)method.Invoke(this, null);
        }

        private DynamicParameterEntry CreateArrayParameterSetupWrapper<T>()
        {
            var type = typeof(T);

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                var setup = new ArrayParameterSetup<T>(this, handler.SetValue, DbType.Object, null);
                return new DynamicParameterEntry(type, (cmd, sql, name, value) =>
                {
                    sql.Append(name);
                    setup.Setup(cmd, name, (T[])value);
                });
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                var setup = new ArrayParameterSetup<T>(this, null, dbType, null);
                return new DynamicParameterEntry(type, (cmd, sql, name, value) =>
                {
                    sql.Append(name);
                    setup.Setup(cmd, name, (T[])value);
                });
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private DynamicParameterEntry CreateListParameterSetupWrapper<T>()
        {
            var type = typeof(T);

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                var setup = new ListParameterSetup<T>(this, handler.SetValue, DbType.Object, null);
                return new DynamicParameterEntry(type, (cmd, sql, name, value) =>
                {
                    sql.Append(name);
                    setup.Setup(cmd, name, (IList<T>)value);
                });
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                var setup = new ListParameterSetup<T>(this, null, dbType, null);
                return new DynamicParameterEntry(type, (cmd, sql, name, value) =>
                {
                    sql.Append(name);
                    setup.Setup(cmd, name, (IList<T>)value);
                });
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private DynamicParameterEntry CreateInParameterSetupWrapper<T>()
        {
            var type = typeof(T);

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                var setup = new InParameterSetup<T>(handler.SetValue, DbType.Object, null);
                return new DynamicParameterEntry(type, (cmd, sql, name, value) =>
                {
                    sql.Append(name);
                    setup.Setup(cmd, name, (T)value);
                });
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                var setup = new InParameterSetup<T>(null, dbType, null);
                return new DynamicParameterEntry(type, (cmd, sql, name, value) =>
                {
                    sql.Append(name);
                    setup.Setup(cmd, name, (T)value);
                });
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        public DynamicParameterSetup CreateDynamicParameterSetup(bool isMultiple)
        {
            return new DynamicParameterSetup(this, isMultiple);
        }
    }
}