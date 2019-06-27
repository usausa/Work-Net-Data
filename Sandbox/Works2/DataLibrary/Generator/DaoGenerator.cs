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
        private const string ImplementSuffix = "_Impl";

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
            var type = typeof(T);
            if (!cache.TryGetValue(type, out var dao))
            {
                dao = cache.AddIfNotExist(type, CreateInternal);
                if (dao == null)
                {
                    throw new AccessorException($"Dao generate failed. type=[{type.FullName}]");
                }
            }

            return (T)dao;
        }

        private object CreateInternal(Type type)
        {
            var cm = new ClassMetadata(type);
            var builder = new CodeBuilder();

            var implementName = cm.Name + ImplementSuffix;

            // Namespace
            builder.BeginNamespace(cm.Namespace);

            // TODO namespaces(block only ?)
            // TODO default helper

            // Class
            builder.BeginClass(implementName, cm.Type);

            // Member
            builder.DefineEngineField();

            // TODO provider if need, and other provider

            // TODO member misc

            // Ctor
            builder.NewLine();
            builder.BeginCtor(implementName);

            // TODO memberInit, provider, convert, parameter

            builder.End();  // Ctor

            foreach (var mm in cm.Methods)
            {
                // Method
                builder.NewLine();
                builder.BeginMethod(mm);

                // TODO open(versions)

                // TODO pre

                // TODO loader and call

                // TODO post

                // dummy
                foreach (var pm in mm.Parameters)
                {
                    if (pm.IsOut)
                    {
                        builder.AppendLine($"{pm.Name} = default;");
                    }
                }

                // dummy
                if (mm.Result.Type != typeof(void))
                {
                    builder.AppendLine("return default;");
                }

                // TODO close(versions)

                builder.End();  // Method
            }

            builder.End();  // Class
            builder.End();  // Namespace

            return Build(cm, builder);
        }

        private object Build(ClassMetadata cm, CodeBuilder builder)
        {
            var source = builder.GetSource();
            var syntax = CSharpSyntaxTree.ParseText(source);

            var references = builder.GetReferences().OrderBy(x => x.FullName).ToArray();
            var metadataReferences = references
                .Select(x => MetadataReference.CreateFromFile(x.Location))
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
                debugger?.Log(cm, source, references);

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

                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                var implementType = assembly.GetType(cm.FullName + ImplementSuffix);
                return Activator.CreateInstance(implementType, engine);
            }
        }
    }
}
