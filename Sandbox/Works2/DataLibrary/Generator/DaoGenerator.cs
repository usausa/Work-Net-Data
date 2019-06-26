namespace DataLibrary.Generator
{
    using System;
    using System.Reflection;

    using DataLibrary.Attributes;
    using DataLibrary.Engine;
    using DataLibrary.Loader;
    using DataLibrary.Parser;

    using Smart.Collections.Concurrent;

    public class DaoGenerator
    {
        private readonly ISqlLoader loader;

        private readonly ExecuteEngine engine;

        private readonly IGeneratorDebugger debugger;

        private readonly ThreadsafeTypeHashArrayMap<object> cache = new ThreadsafeTypeHashArrayMap<object>();

        public DaoGenerator(ISqlLoader loader, ExecuteEngine engine)
            : this(loader, engine, null)
        {
        }

        public DaoGenerator(ISqlLoader loader, ExecuteEngine engine, IGeneratorDebugger debugger)
        {
            this.loader = loader;
            this.engine = engine;
            this.debugger = debugger;
        }

        public T Create<T>()
        {
            if (!cache.TryGetValue(typeof(T), out var dao))
            {
                dao = cache.AddIfNotExist(typeof(T), CreateInternal);
            }

            return (T)dao;
        }

        private object CreateInternal(Type type)
        {
            var classData = new ClassMetadata(type);
            var builder = new CodeBuilder();

            // Generation
            builder.BeginNamespace(classData.Namespace);

            // TODO コード生成(ここがメインか)

            builder.EndNamespace();

            // Build
            var source = builder.ToSource();
            // TODO ビルド

            // TODO インスタンス化

            return default;
        }
    }
}
