using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ResultMapperCacheWork
{
    class Program
    {
        static void Main()
        {
        }
    }

    public sealed class Engine
    {
        public Func<IDataRecord, T> CreateMapper<T>(ColumnInfo[] columns)
        {
            return x => Activator.CreateInstance<T>();
        }
    }

    public static class ThreadLocalCache
    {
        [ThreadStatic]
        public static ColumnInfo[] ColumnInfoPool;
    }

    public sealed class ResultMapperCache<T>
    {

        private readonly Engine engine;

        private readonly object sync = new object();

        private Node[] nodes = Array.Empty<Node>();

        public ResultMapperCache(Engine engine)
        {
            this.engine = engine;
        }

        public Func<IDataRecord, T> ResolveMapper(IDataReader reader)
        {
            // TODO stackalloc?
            var fieldCount = reader.FieldCount;
            if ((ThreadLocalCache.ColumnInfoPool == null) || (ThreadLocalCache.ColumnInfoPool.Length < fieldCount))
            {
                ThreadLocalCache.ColumnInfoPool = new ColumnInfo[fieldCount];
            }

            var type = typeof(T);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                ThreadLocalCache.ColumnInfoPool[i] = new ColumnInfo(reader.GetName(i), reader.GetFieldType(i));
            }

            var columns = new Span<ColumnInfo>(ThreadLocalCache.ColumnInfoPool, 0, fieldCount);
            var mapper = FindMapper(columns);
            if (mapper != null)
            {
                return mapper;
            }

            return AddIfNotExist(columns);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Func<IDataRecord, T> FindMapper(Span<ColumnInfo> columns)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                if (IsMatchColumn(node.Columns, columns))
                {
                    return node.Value;
                }
            }

            return null;
        }

        private Func<IDataRecord, T> AddIfNotExist(Span<ColumnInfo> columns)
        {
            lock (sync)
            {
                // Double checked locking
                var mapper = FindMapper(columns);
                if (mapper != null)
                {
                    return mapper;
                }

                var copyColumns = new ColumnInfo[columns.Length];
                columns.CopyTo(new Span<ColumnInfo>(copyColumns));

                mapper = engine.CreateMapper<T>(copyColumns);

                AddNode(copyColumns, mapper);

                return mapper;
            }
        }

        private void AddNode(ColumnInfo[] columns, Func<IDataRecord, T> mapper)
        {
            // TODO

            Interlocked.MemoryBarrier();

            // TODO
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMatchColumn(ColumnInfo[] columns1, Span<ColumnInfo> columns2)
        {
            if (columns1.Length != columns2.Length)
            {
                return false;
            }

            for (var i = 0; i < columns1.Length; i++)
            {
                var column1 = columns1[i];
                var column2 = columns2[i];

                if (column1.Type != column2.Type)
                {
                    return false;
                }

                if (String.Compare(column1.Name, column2.Name, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }
            }

            return true;
        }


        private readonly struct Node
        {
            public readonly ColumnInfo[] Columns;

            public readonly Func<IDataRecord, T> Value;

            public Node(ColumnInfo[] columns, Func<IDataRecord, T> value)
            {
                Columns = columns;
                Value = value;
            }
        }
    }

    public readonly struct ColumnInfo
    {
        public readonly string Name;

        public readonly Type Type;

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }}
