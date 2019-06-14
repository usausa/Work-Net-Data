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

    public sealed class ExecuteEngine : IEngineController
    {
        private const CommandBehavior CommandBehaviorForEnumerable =
            CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorForEnumerableWithClose =
            CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection;

        private const CommandBehavior CommandBehaviorForList =
            CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorForSingle =
            CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        private readonly IComponentContainer container;

        private readonly IObjectConverter converter;

        private readonly Dictionary<Type, DbType> typeMap;

        private readonly Dictionary<Type, ITypeHandler> typeHandlers;

        private readonly IResultMapperFactory[] resultMapperFactories;

        private readonly ResultMapperCache resultMapperCache = new ResultMapperCache();

        [ThreadStatic]
        private static ColumnInfo[] columnInfoPool;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public ExecuteEngine(IExecuteEngineConfig config)
        {
            container = config.CreateComponentContainer();
            converter = container.Get<IObjectConverter>();
            typeMap = new Dictionary<Type, DbType>(config.GetTypeMap());
            typeHandlers = new Dictionary<Type, ITypeHandler>(config.GetTypeHandlers());
            resultMapperFactories = config.GetResultMapperFactories();
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
        // ResultMapper
        //--------------------------------------------------------------------------------

        private Func<IDataRecord, T> CreateResultMapper<T>(IDataReader reader)
        {
            var fieldCount = reader.FieldCount;
            if ((columnInfoPool == null) || (columnInfoPool.Length < fieldCount))
            {
                columnInfoPool = new ColumnInfo[fieldCount];
            }

            var type = typeof(T);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                columnInfoPool[i] = new ColumnInfo(reader.GetName(i), reader.GetFieldType(i));
            }

            var columns = new Span<ColumnInfo>(columnInfoPool, 0, fieldCount);

            if (resultMapperCache.TryGetValue(type, columns, out var value))
            {
                return (Func<IDataRecord, T>)value;
            }

            return (Func<IDataRecord, T>)resultMapperCache.AddIfNotExist(type, columns, CreateMapperInternal<T>);
        }

        private object CreateMapperInternal<T>(Type type, ColumnInfo[] columns)
        {
            foreach (var factory in resultMapperFactories)
            {
                if (factory.IsMatch(type))
                {
                    return factory.CreateMapper<T>(container, type, columns);
                }
            }

            throw new AccessorException($"Result type is not supported. type=[{type.FullName}]");
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Execute(DbConnection con, DbCommand cmd)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }

            return cmd.ExecuteNonQuery();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<int> ExecuteAsync(DbConnection con, DbCommand cmd, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ExecuteScalar<T>(DbCommand cmd)
        {
            var result = cmd.ExecuteScalar();

            return Convert<T>(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<T> ExecuteScalarAsync<T>(DbCommand cmd, CancellationToken cancel = default)
        {
            var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

            return Convert<T>(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ExecuteScalar<T>(DbConnection con, DbCommand cmd)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return ExecuteScalar<T>(cmd);
                }
                finally
                {
                    con.Close();
                }
            }

            return ExecuteScalar<T>(cmd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<T> ExecuteScalarAsync<T>(DbConnection con, DbCommand cmd, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await ExecuteScalarAsync<T>(cmd, cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await ExecuteScalarAsync<T>(cmd, cancel).ConfigureAwait(false);
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbDataReader ExecuteReader(DbCommand cmd)
        {
            return cmd.ExecuteReader(CommandBehaviorForEnumerableWithClose);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<DbDataReader> ExecuteReaderAsync(DbCommand cmd, CancellationToken cancel)
        {
            return cmd.ExecuteReaderAsync(CommandBehaviorForEnumerableWithClose, cancel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DbDataReader ExecuteReader(DbCommand cmd, bool withClose)
        {
            return cmd.ExecuteReader(withClose ? CommandBehaviorForEnumerableWithClose : CommandBehaviorForEnumerable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<DbDataReader> ExecuteReaderAsync(DbCommand cmd, bool withClose, CancellationToken cancel)
        {
            return cmd.ExecuteReaderAsync(withClose ? CommandBehaviorForEnumerableWithClose : CommandBehaviorForEnumerable, cancel);
        }

        //--------------------------------------------------------------------------------
        // ReaderToDefer
        //--------------------------------------------------------------------------------

        public IEnumerable<T> ReaderToDefer<T>(IDbCommand cmd, IDataReader reader)
        {
            var mapper = CreateResultMapper<T>(reader);

            using (cmd)
            using (reader)
            {
                while (reader.Read())
                {
                    yield return mapper(reader);
                }
            }
        }

        //--------------------------------------------------------------------------------
        // QueryBuffer
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IList<T> QueryBuffer<T>(DbCommand cmd)
        {
            using (var reader = cmd.ExecuteReader(CommandBehaviorForList))
            {
                var mapper = CreateResultMapper<T>(reader);

                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(mapper(reader));
                }

                return list;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IList<T>> QueryBufferAsync<T>(DbCommand cmd, CancellationToken cancel = default)
        {
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorForList, cancel).ConfigureAwait(false))
            {
                var mapper = CreateResultMapper<T>(reader);

                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(mapper(reader));
                }

                return list;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IList<T> QueryBuffer<T>(DbConnection con, DbCommand cmd)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return QueryBuffer<T>(cmd);
                }
                finally
                {
                    con.Close();
                }
            }


            return QueryBuffer<T>(cmd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IList<T>> QueryBufferAsync<T>(DbConnection con, DbCommand cmd, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await QueryBufferAsync<T>(cmd, cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await QueryBufferAsync<T>(cmd, cancel).ConfigureAwait(false);
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T QueryFirstOrDefault<T>(DbCommand cmd)
        {
            using (var reader = cmd.ExecuteReader(CommandBehaviorForSingle))
            {
                var mapper = CreateResultMapper<T>(reader);
                return reader.Read() ? mapper(reader) : default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<T> QueryFirstOrDefaultAsync<T>(DbCommand cmd, CancellationToken cancel = default)
        {
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorForSingle, cancel))
            {
                var mapper = CreateResultMapper<T>(reader);
                return reader.Read() ? mapper(reader) : default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T QueryFirstOrDefault<T>(DbConnection con, DbCommand cmd)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    con.Open();

                    return QueryFirstOrDefault<T>(cmd);
                }
                finally
                {
                    con.Close();
                }
            }

            return QueryFirstOrDefault<T>(cmd);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<T> QueryFirstOrDefaultAsync<T>(DbConnection con, DbCommand cmd, CancellationToken cancel = default)
        {
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await QueryFirstOrDefaultAsync<T>(cmd, cancel).ConfigureAwait(false);
                }
                finally
                {
                    con.Close();
                }
            }

            return await QueryFirstOrDefaultAsync<T>(cmd, cancel).ConfigureAwait(false);
        }
    }
}
