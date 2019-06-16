using System.Linq;
using DataLibrary.Dialect;
using DataLibrary.Namings;

namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;

    using DataLibrary.Handlers;
    using DataLibrary.Mappers;

    using Smart.Converter;
    using Smart.ComponentModel;

    public sealed partial class ExecuteEngine : IEngineController
    {
        private readonly IComponentContainer container;

        private readonly IObjectConverter converter;

        private readonly IEmptyDialect emptyDialect;

        private readonly IParameterNaming parameterNaming;

        private readonly Dictionary<Type, DbType> typeMap;

        private readonly Dictionary<Type, ITypeHandler> typeHandlers;

        private readonly IResultMapperFactory[] resultMapperFactories;

        private readonly ResultMapperCache resultMapperCache = new ResultMapperCache();

        private readonly string[] parameterNames;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public ExecuteEngine(IExecuteEngineConfig config)
        {
            container = config.CreateComponentContainer();
            converter = container.Get<IObjectConverter>();
            emptyDialect = container.Get<IEmptyDialect>();
            parameterNaming = container.Get<IParameterNaming>();

            typeMap = new Dictionary<Type, DbType>(config.GetTypeMap());
            typeHandlers = new Dictionary<Type, ITypeHandler>(config.GetTypeHandlers());
            resultMapperFactories = config.GetResultMapperFactories();

            parameterNames = Enumerable.Range(0, 256).Select(x => parameterNaming.GetName(x)).ToArray();
        }

        //--------------------------------------------------------------------------------
        // Controller
        //--------------------------------------------------------------------------------

        public int CountResultMapperCache => resultMapperCache.Count;

        public void ClearResultMapperCache() => resultMapperCache.Clear();

        //--------------------------------------------------------------------------------
        // Component
        //--------------------------------------------------------------------------------

        public T GetComponent<T>() => container.Get<T>();

        // TODO ?
        public T GetTypeHandler<T>() where T : ITypeHandler
        {
            // TODO
            return Activator.CreateInstance<T>();
        }

        // TODO TypeMap / CommandBuilder (TypeHandler integration ?)

        //Func<object, object> ISqlMapperConfig.CreateParser(Type sourceType, Type destinationType)
        //{
        //    if (!typeHandleEntriesCache.TryGetValue(destinationType, out var entry))
        //    {
        //        entry = typeHandleEntriesCache.AddIfNotExist(destinationType, CreateTypeHandleInternal);
        //    }

        //    if (entry.TypeHandler != null)
        //    {
        //        return x => entry.TypeHandler.Parse(destinationType, x);
        //    }

        //    return Converter.CreateConverter(sourceType, destinationType);
        //}

        //-----------
        //TypeHandleEntry ISqlMapperConfig.LookupTypeHandle(Type type)
        //{
        //    if (!typeHandleEntriesCache.TryGetValue(type, out var entry))
        //    {
        //        entry = typeHandleEntriesCache.AddIfNotExist(type, CreateTypeHandleInternal);
        //    }

        //    if (!entry.CanUseAsParameter)
        //    {
        //        throw new SqlMapperException($"Type cannot use as parameter. type=[{type.FullName}]");
        //    }

        //    return entry;
        //}

        //private TypeHandleEntry CreateTypeHandleInternal(Type type)
        //{
        //    type = Nullable.GetUnderlyingType(type) ?? type;
        //    var findDbType = typeMap.TryGetValue(type, out var dbType);
        //    if (!findDbType && type.IsEnum)
        //    {
        //        findDbType = typeMap.TryGetValue(Enum.GetUnderlyingType(type), out dbType);
        //    }

        //    typeHandlers.TryGetValue(type, out var handler);

        //    return new TypeHandleEntry(findDbType || (handler != null), dbType, handler);
        //}

        //--------------------------------------------------------------------------------
        // Converter
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Convert<T>(object value)
        {
            if (value is T scalar)
            {
                return scalar;
            }

            if (value is DBNull)
            {
                return default;
            }

            // TODO TypeHandlerも含めてキャッシュにするか！、ルックアップ1回で！

            //return (T)System.Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)converter.Convert(value, typeof(T));
        }

        //--------------------------------------------------------------------------------
        // Naming
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetParameterName(int index)
        {
            return index < parameterNames.Length ? parameterNames[index] : parameterNaming.GetName(index);
        }
    }
}
