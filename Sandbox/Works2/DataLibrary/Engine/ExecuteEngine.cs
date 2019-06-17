namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using DataLibrary.Dialect;
    using DataLibrary.Handlers;
    using DataLibrary.Mappers;
    using DataLibrary.Namings;

    using Smart.Converter;
    using Smart.ComponentModel;

    public sealed partial class ExecuteEngine : IEngineController
    {
        private readonly IComponentContainer container;

        private readonly IObjectConverter objectConverter;

        private readonly IEmptyDialect emptyDialect;

        private readonly IParameterNaming parameterNaming;

        private readonly Dictionary<Type, DbType> typeMap;

        private readonly Dictionary<Type, ITypeHandler> typeHandlers;

        private readonly IResultMapperFactory[] resultMapperFactories;

        private readonly ResultMapperCache resultMapperCache = new ResultMapperCache();

        private readonly string[] parameterNames;

        private readonly string[] parameterSubNames;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public ExecuteEngine(IExecuteEngineConfig config)
        {
            container = config.CreateComponentContainer();
            objectConverter = container.Get<IObjectConverter>();
            emptyDialect = container.Get<IEmptyDialect>();
            parameterNaming = container.Get<IParameterNaming>();

            typeMap = new Dictionary<Type, DbType>(config.GetTypeMap());
            typeHandlers = new Dictionary<Type, ITypeHandler>(config.GetTypeHandlers());
            resultMapperFactories = config.GetResultMapperFactories();

            parameterNames = Enumerable.Range(0, 256).Select(x => parameterNaming.GetName(x)).ToArray();
            parameterSubNames = Enumerable.Range(0, 256).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
        }

        //--------------------------------------------------------------------------------
        // Component
        //--------------------------------------------------------------------------------

        public T GetComponent<T>() => container.Get<T>();

        //--------------------------------------------------------------------------------
        // Controller
        //--------------------------------------------------------------------------------

        public int CountResultMapperCache => resultMapperCache.Count;

        public void ClearResultMapperCache() => resultMapperCache.Clear();

        //--------------------------------------------------------------------------------
        // Lookup
        //--------------------------------------------------------------------------------

        private bool LookupTypeHandler(Type type, out ITypeHandler handler)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (typeHandlers.TryGetValue(type, out handler))
            {
                return true;
            }

            if (type.IsEnum && typeHandlers.TryGetValue(Enum.GetUnderlyingType(type), out handler))
            {
                return true;
            }

            handler = null;
            return false;
        }

        private bool LookupDbType(Type type, out DbType dbType)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (typeMap.TryGetValue(type, out dbType))
            {
                return true;
            }

            if (type.IsEnum && typeMap.TryGetValue(Enum.GetUnderlyingType(type), out dbType))
            {
                return true;
            }

            dbType = DbType.Object;
            return false;
        }

        // TODO Enumも考慮して、TypeHandler or

        //--------------------------------------------------------------------------------
        // Converter
        //--------------------------------------------------------------------------------

        public Func<object, object> CreateConverter<T>(ICustomAttributeProvider provider)
        {
            var type = typeof(T);

            // TODO ResultAttribute

            // ITypeHandler
            if (LookupTypeHandler(type, out var handler))
            {
                return x => handler.Parse(type, x);
            }

            return CreateConverterByObjectConverter(Nullable.GetUnderlyingType(type) ?? type);
        }

        private Func<object, object> CreateConverterByObjectConverter(Type type)
        {
            return x =>
            {
                var converter = objectConverter.CreateConverter(x.GetType(), type);
                return converter(x);
            };
        }

        //--------------------------------------------------------------------------------
        // Naming
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetParameterName(int index)
        {
            return index < parameterNames.Length ? parameterNames[index] : parameterNaming.GetName(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetParameterSubName(int index)
        {
            return index < parameterSubNames.Length ? parameterSubNames[index] : index.ToString(CultureInfo.InvariantCulture);
        }
    }
}
