using System.Linq;
using System.Reflection;
using System.Text;
using DataLibrary.Attributes;
using DataLibrary.Dialect;

namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using DataLibrary.Handlers;
    using DataLibrary.Mappers;

    using Smart.Converter;
    using Smart.ComponentModel;

    public sealed partial class ExecuteEngine
    {
        //--------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------

        public Action<DbCommand, string, object> CreateInParameterSetup(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return ParameterSetupHelper.CreateInParameterSetupByAction(attribute.CreateSetAction());
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return ParameterSetupHelper.CreateInParameterSetupByAction(handler.SetValue);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return ParameterSetupHelper.CreateInParameterSetupByDbType(dbType);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        public Func<DbCommand, string, object, DbParameter> CreateInOutParameterSetup(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return ParameterSetupHelper.CreateInOutParameterSetupByAction(attribute.CreateSetAction());
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return ParameterSetupHelper.CreateInOutParameterSetupByAction(handler.SetValue);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return ParameterSetupHelper.CreateInOutParameterSetupByDbType(dbType);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        public Func<DbCommand, string, DbParameter> CreateOutParameterSetup(ParameterDirection direction)
        {
            return ParameterSetupHelper.CreateOutParameterSetup(direction);
        }

        public Action<DbCommand, string, StringBuilder, T[]> CreateArrayParameterSetup<T>(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return ParameterSetupHelper.CreateArrayParameterSetupByAction<T>(attribute.CreateSetAction(), emptyDialect);
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return ParameterSetupHelper.CreateArrayParameterSetupByAction<T>(handler.SetValue, emptyDialect);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return ParameterSetupHelper.CreateArrayParameterSetupByDbType<T>(dbType, emptyDialect);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        public Action<DbCommand, string, StringBuilder, IList<T>> CreateListParameterSetup<T>(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return ParameterSetupHelper.CreateListParameterSetupByAction<T>(attribute.CreateSetAction(), emptyDialect);
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return ParameterSetupHelper.CreateListParameterSetupByAction<T>(handler.SetValue, emptyDialect);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return ParameterSetupHelper.CreateListParameterSetupByDbType<T>(dbType, emptyDialect);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }

        public Action<DbCommand, string, StringBuilder, IEnumerable<T>> CreateEnumerableParameterSetup<T>(Type type, ICustomAttributeProvider provider)
        {
            // ParameterAttribute
            var attribute = provider.GetCustomAttributes(true).Cast<ParameterAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return ParameterSetupHelper.CreateEnumerableParameterSetupByAction<T>(attribute.CreateSetAction(), emptyDialect);
            }

            // ITypeHandler
            if (typeHandlers.TryGetValue(type, out var handler))
            {
                return ParameterSetupHelper.CreateEnumerableParameterSetupByAction<T>(handler.SetValue, emptyDialect);
            }

            // Type
            if (typeMap.TryGetValue(type, out var dbType))
            {
                return ParameterSetupHelper.CreateEnumerableParameterSetupByDbType<T>(dbType, emptyDialect);
            }

            throw new AccessorException($"Parameter type is not supported. type=[{type.FullName}]");
        }
    }
}
