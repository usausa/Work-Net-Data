namespace Smart.Data.Accessor.Engine
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Helpers;

    public sealed partial class ExecuteEngine
    {
        //--------------------------------------------------------------------------------
        // In
        //--------------------------------------------------------------------------------

        public sealed class InParameterSetup
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

            public void Setup(DbCommand cmd, string name, object value)
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

        public InParameterSetup CreateInParameterSetup(Type type, ICustomAttributeProvider provider)
        {
            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new InParameterSetup(null, attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new InParameterSetup(handler.SetValue, DbType.Object, null);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new InParameterSetup(null, dbType, null);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // In/Out
        //--------------------------------------------------------------------------------

        public sealed class InOutParameterSetup
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

            public DbParameter Setup(DbCommand cmd, string name, object value)
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

        public InOutParameterSetup CreateInOutParameterSetup(Type type, ICustomAttributeProvider provider)
        {
            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new InOutParameterSetup(null, attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new InOutParameterSetup(handler.SetValue, DbType.Object, null);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new InOutParameterSetup(null, dbType, null);
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

        public OutParameterSetup CreateOutParameterSetup(Type type, ICustomAttributeProvider provider)
        {
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
        // IList
        //--------------------------------------------------------------------------------

        public sealed class ListParameterSetup
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

            public void AppendSql(StringBuilder sql, string name, IList values)
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

            public void Setup(DbCommand cmd, string name, IList values)
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

        public ListParameterSetup CreateListParameterSetup(Type type, ICustomAttributeProvider provider)
        {
            // ParameterBuilderAttribute
            var attribute = provider.GetCustomAttributes(true).OfType<ParameterBuilderAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return new ListParameterSetup(this, null, attribute.DbType, attribute.Size);
            }

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new ListParameterSetup(this, handler.SetValue, DbType.Object, null);
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new ListParameterSetup(this, null, dbType, null);
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // Dynamic
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Performance")]
        private sealed class DynamicParameterEntry
        {
            public static DynamicParameterEntry Empty { get; } = new DynamicParameterEntry(null, null);

            public readonly Type Type;

            public readonly Action<DbCommand, StringBuilder, string, object> Handler;

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
            if (ParameterHelper.IsMultipleParameter(type))
            {
                var method = GetType().GetMethod(nameof(CreateDynamicListParameterSetup), BindingFlags.Instance | BindingFlags.NonPublic);
                var elementType = ParameterHelper.GetMultipleParameterElementType(type);
                return (DynamicParameterEntry)method.Invoke(this, new object[] { elementType });
            }
            else
            {
                var method = GetType().GetMethod(nameof(CreateDynamicSimpleParameterSetup), BindingFlags.Instance | BindingFlags.NonPublic);
                return (DynamicParameterEntry)method.Invoke(this, new object[] { type });
            }
        }

        private DynamicParameterEntry CreateDynamicListParameterSetup(Type type)
        {
            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new DynamicParameterEntry(type, CreateDynamicListParameterHandler(handler.SetValue, DbType.Object));
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new DynamicParameterEntry(type, CreateDynamicListParameterHandler(null, dbType));
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private Action<DbCommand, StringBuilder, string, object> CreateDynamicListParameterHandler(Action<DbParameter, object> handler, DbType dbType)
        {
            return (cmd, sql, name, value) =>
            {
                var values = (IList)value;

                sql.Append("(");

                if (values.Count == 0)
                {
                    sql.Append(emptyDialect.GetSql());
                }
                else
                {
                    for (var i = 0; i < values.Count; i++)
                    {
                        sql.Append(name);
                        sql.Append(GetParameterSubName(i));
                        sql.Append(", ");
                    }

                    sql.Length -= 2;
                }

                sql.Append(") ");

                for (var i = 0; i < values.Count; i++)
                {
                    var parameter = cmd.CreateParameter();
                    cmd.Parameters.Add(parameter);
                    if (handler != null)
                    {
                        handler(parameter, values[i]);
                    }
                    else
                    {
                        parameter.Value = values[i];
                        parameter.DbType = dbType;
                    }
                    parameter.ParameterName = name + GetParameterSubName(i);
                }
            };
        }

        private DynamicParameterEntry CreateDynamicSimpleParameterSetup(Type type)
        {
            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return new DynamicParameterEntry(type, CreateDynamicSimpleParameterHandler(handler.SetValue, DbType.Object));
            }

            // Type
            if (LookupDbType(type, out var dbType))
            {
                return new DynamicParameterEntry(type, CreateDynamicSimpleParameterHandler(null, dbType));
            }

            throw new AccessorRuntimeException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        private static Action<DbCommand, StringBuilder, string, object> CreateDynamicSimpleParameterHandler(Action<DbParameter, object> handler, DbType dbType)
        {
            return (cmd, sql, name, value) =>
            {
                sql.Append(name);

                var parameter = cmd.CreateParameter();
                cmd.Parameters.Add(parameter);
                if (handler != null)
                {
                    handler(parameter, value);
                }
                else
                {
                    parameter.Value = value;
                    parameter.DbType = dbType;
                }
                parameter.ParameterName = name;
            };
        }

        public DynamicParameterSetup CreateDynamicParameterSetup(bool isMultiple)
        {
            return new DynamicParameterSetup(this, isMultiple);
        }
    }
}