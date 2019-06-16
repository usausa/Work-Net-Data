﻿namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DataLibrary.Dialect;
    using DataLibrary.Handlers;
    using DataLibrary.Mappers;
    using DataLibrary.Namings;
    using DataLibrary.Selectors;

    using Smart.ComponentModel;
    using Smart.Converter;
    using Smart.Reflection;

    public sealed class ExecuteEngineConfig : IExecuteEngineConfig
    {
        private readonly Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>
        {
            { typeof(byte), DbType.Byte },
            { typeof(sbyte), DbType.SByte },
            { typeof(short), DbType.Int16 },
            { typeof(ushort), DbType.UInt16 },
            { typeof(int), DbType.Int32 },
            { typeof(uint), DbType.UInt32 },
            { typeof(long), DbType.Int64 },
            { typeof(ulong), DbType.UInt64 },
            { typeof(float), DbType.Single },
            { typeof(double), DbType.Double },
            { typeof(decimal), DbType.Decimal },
            { typeof(bool), DbType.Boolean },
            { typeof(string), DbType.String },
            { typeof(char), DbType.StringFixedLength },
            { typeof(Guid), DbType.Guid },
            { typeof(DateTime), DbType.DateTime },
            { typeof(DateTimeOffset), DbType.DateTimeOffset },
            { typeof(TimeSpan), DbType.Time },
            { typeof(byte[]), DbType.Binary },
            { typeof(object), DbType.Object },
        };

        private readonly Dictionary<Type, ITypeHandler> typeHandlers = new Dictionary<Type, ITypeHandler>();

        private readonly List<IResultMapperFactory> resultMapperFactories = new List<IResultMapperFactory>
        {
            ObjectResultMapperFactory.Instance
        };

        //--------------------------------------------------------------------------------
        // Config
        //--------------------------------------------------------------------------------

        public ComponentConfig Components { get; } = new ComponentConfig();

        public IDictionary<Type, DbType> TypeMap => typeMap;

        public IDictionary<Type, ITypeHandler> TypeHandlers => typeHandlers;

        public IList<IResultMapperFactory> ResultMapperFactories => resultMapperFactories;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public ExecuteEngineConfig()
        {
            Components.Add<IDelegateFactory>(DelegateFactory.Default);
            Components.Add<IObjectConverter>(ObjectConverter.Default);
            Components.Add<IPropertySelector>(DefaultPropertySelector.Instance);
            Components.Add<IParameterNaming>(new ParameterNaming("@"));
            Components.Add<IEmptyDialect, EmptyDialect>();
        }

        //--------------------------------------------------------------------------------
        // Interface
        //--------------------------------------------------------------------------------

        IComponentContainer IExecuteEngineConfig.CreateComponentContainer() => Components.ToContainer();

        public IDictionary<Type, DbType> GetTypeMap() => typeMap;

        public IDictionary<Type, ITypeHandler> GetTypeHandlers() => typeHandlers;

        IResultMapperFactory[] IExecuteEngineConfig.GetResultMapperFactories() => resultMapperFactories.ToArray();
    }
}
