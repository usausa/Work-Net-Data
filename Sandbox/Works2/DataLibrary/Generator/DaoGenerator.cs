namespace DataLibrary.Generator
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using DataLibrary.Engine;
    using DataLibrary.Loader;

    using Smart.Collections.Concurrent;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

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

            // TODO namespaces(block only ?)
            // TODO default helper

            builder.BeginClass($"{type.Name}_Impl", type.FullName);

            // TODO member engine
            // TODO member misc
            // TODO ctor
            // TODO methods
            // TODO loader

            builder.End();  // class
            builder.End();  // namespace

            // Build
            return Build(type, builder.ToSource());
        }

        private object Build(Type type, string source)
        {
            var syntax = CSharpSyntaxTree.ParseText(source);

            var metadataReferences = type.Assembly.GetReferencedAssemblies()
                .Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location))
                .ToArray();

            var assemblyName = Path.GetRandomFileName();

            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release);

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntax },
                metadataReferences,
                options);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                // TODO Debug report
                debugger?.Log(type, source);

                if (!result.Success)
                {
                    // TODO Error
                    var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
                    foreach (var failure in failures)
                    {
                        Debug.WriteLine(
                            "{0} [{1}...{2}] {3}",
                            failure.Id,
                            failure.Location.SourceSpan.Start,
                            failure.Location.SourceSpan.End,
                            failure.GetMessage());
                    }

                    throw new AccessorException($"Dao generate failed. type=[{type.FullName}]");
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                var implementType = assembly.GetType($"{type.FullName}_Impl");
                return Activator.CreateInstance(implementType, engine);
            }
        }
    }
}
