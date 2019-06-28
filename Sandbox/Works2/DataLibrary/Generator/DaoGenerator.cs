namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using DataLibrary.Attributes;
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

        private static readonly HashSet<Assembly> DefaultReference = new HashSet<Assembly>();

        static DaoGenerator()
        {
            AddReference(DefaultReference, typeof(ExecuteEngine).Assembly);
            //foreach (var assembly in DefaultReference.OrderBy(x => x.FullName))
            //{
            //    System.Diagnostics.Debug.WriteLine(assembly);
            //}
        }

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

        private static void AddReference(HashSet<Assembly> assemblies, Assembly assembly)
        {
            if (assemblies.Contains(assembly))
            {
                return;
            }

            assemblies.Add(assembly);

            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                AddReference(assemblies, Assembly.Load(assemblyName));
            }
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
            if (type.GetCustomAttribute<DaoAttribute>() is null)
            {
                throw new AccessorException($"Type is not supported for generation. type=[{type.FullName}]");
            }

            var builder = new DaoSourceBuilder(type);

            var no = 1;
            foreach (var method in type.GetMethods())
            {
                var attribute = method.GetCustomAttribute<MethodAttribute>(true);
                if (attribute == null)
                {
                    throw new AccessorException($"Method is not supported for generation. type=[{type.FullName}], method=[{method.Name}]");
                }

                var blocks = attribute.GetBlocks(loader, method);
                var methodMetadata = new MethodMetadata(no, method, attribute.CommandType, attribute.MethodType, blocks);

                switch (methodMetadata.MethodType)
                {
                    case MethodType.Execute:
                        if (!IsValidExecuteResultType(methodMetadata.EngineResultType))
                        {
                            throw new AccessorException($"ReturnType is not match for MethodType.Execute. type=[{type.FullName}], method=[{method.Name}], returnType=[{method.ReturnType}]");
                        }
                        break;
                    case MethodType.Query:
                        if (!IsValidQueryResultType(methodMetadata.EngineResultType))
                        {
                            throw new AccessorException($"ReturnType is not match for MethodType.Query. type=[{type.FullName}], method=[{method.Name}], returnType=[{method.ReturnType}]");
                        }
                        break;
                }

                builder.AddMethod(methodMetadata);

                no++;
            }

            return Build(builder.Build());
        }

        private static bool IsValidExecuteResultType(Type type)
        {
            return type == typeof(int) || type == typeof(void);
        }

        private static bool IsValidQueryResultType(Type type)
        {
            return type != typeof(string) &&
                   type.GetInterfaces().Prepend(type).Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        private object Build(DaoSource source)
        {
            var syntax = CSharpSyntaxTree.ParseText(source.Code);

            var references = new HashSet<Assembly>(DefaultReference);
            foreach (var assembly in source.References)
            {
                AddReference(references, assembly);
            }

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

                debugger?.Log(
                    result.Success,
                    source,
                    result.Diagnostics
                        .Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                        .Select(x => new BuildError(x.Id, x.Location.SourceSpan.Start, x.Location.SourceSpan.End, x.GetMessage()))
                        .ToArray());

                if (!result.Success)
                {
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                var implementType = assembly.GetType(source.ImplementTypeFullName);
                return Activator.CreateInstance(implementType, engine);
            }
        }
    }
}
