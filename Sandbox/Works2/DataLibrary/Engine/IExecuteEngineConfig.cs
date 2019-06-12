namespace DataLibrary.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DataLibrary.Handlers;
    using DataLibrary.Mappers;

    using Smart.ComponentModel;

    public interface IExecuteEngineConfig
    {
        IComponentContainer CreateComponentContainer();

        IDictionary<Type, DbType> GetTypeMap();

        IDictionary<Type, ITypeHandler> GetTypeHandlers();

        IResultMapperFactory[] GetResultMapperFactories();
    }
}
