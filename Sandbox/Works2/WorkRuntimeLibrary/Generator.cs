namespace WorkRuntimeLibrary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class Generator
    {
        private readonly Engine engine = new Engine();

        private readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();

        public T Create<T>()
        {
            var type = typeof(T);
            if (!type.IsInterface)
            {
                return default;
            }

            lock (cache)
            {
                if (!cache.TryGetValue(type, out var obj))
                {
                    obj = Generate(type);
                    if (obj == null)
                    {
                        return default;
                    }

                    cache[type] = obj;
                }

                return (T)obj;
            }
        }

        private object Generate(Type type)
        {
            var references = new HashSet<Assembly>
            {
                typeof(object).Assembly,
                typeof(Engine).Assembly,
                type.Assembly
            };
            var source = new StringBuilder();

            source.AppendLine($"namespace {type.Namespace}");
            source.AppendLine("{");
            source.AppendLine($"internal sealed class {type.Name}_Impl : global::{type.FullName}");
            source.AppendLine("{");
            source.AppendLine("    private readonly global::WorkRuntimeLibrary.Engine engine;");
            source.AppendLine();
            source.AppendLine($"    public {type.Name}_Impl(global::WorkRuntimeLibrary.Engine engine)");
            source.AppendLine("    {");
            source.AppendLine("        this.engine = engine;");
            source.AppendLine("    }");

            foreach (var mi in type.GetMethods())
            {
                references.Add(mi.ReturnType.Assembly);

                source.AppendLine();
                source.Append($"    public global::{mi.ReturnType.FullName} {mi.Name}(");

                foreach (var parameter in mi.GetParameters())
                {
                    references.Add(parameter.ParameterType.Assembly);

                    source.Append($"global::{parameter.ParameterType.FullName} {parameter.Name}, ");
                }

                if (mi.GetParameters().Length > 0)
                {
                    source.Length -= 2;
                }

                source.AppendLine(")");
                source.AppendLine("    {");

                if (mi.ReturnType != typeof(void))
                {
                    source.Append($"        return this.engine.{mi.Name}(");

                    foreach (var parameter in mi.GetParameters())
                    {
                        source.Append($"{parameter.Name}, ");
                    }

                    if (mi.GetParameters().Length > 0)
                    {
                        source.Length -= 2;
                    }

                    source.AppendLine(");");
                }

                source.AppendLine("    }");
            }

            source.AppendLine("}");
            source.AppendLine();
            source.AppendLine("}");

            var syntax = CSharpSyntaxTree.ParseText(source.ToString());

            var systemRuntime = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location);
            var netStandard = MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location);

            var metadataReferences = references
                .Select(x => MetadataReference.CreateFromFile(x.Location))
                .Concat(new [] { systemRuntime, netStandard })
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
                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
                    foreach (var failure in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", failure.Id, failure.GetMessage());
                    }

                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());

                var implType = assembly.GetType($"{type.FullName}_Impl");
                return Activator.CreateInstance(implType, engine);
            }
        }
    }
}
