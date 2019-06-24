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
            if (type.GetCustomAttribute<DaoAttribute>() is null)
            {
                throw new AccessorException($"Type is not supported for generation. type=[{type.FullName}]");
            }

            var classData = new ClassMetadata(type);
            foreach (var method in type.GetMethods())
            {
                var attribute = method.GetCustomAttribute<MethodAttribute>(true);
                if (attribute == null)
                {
                    throw new AccessorException($"Method is not supported for generation. type=[{type.FullName}], method=[{method.Name}]");
                }

                // TODO 属性チェック
                // 分析＆追加

                classData.Methods.Add(new MethodMetadata(method));
            }

            // TODO コード生成(ここがメインか)

            // TODO ビルド

            // TODO インスタンス化

            return default;
        }
    }
}
