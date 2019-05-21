namespace WorkLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    public class Generator
    {
        private readonly ScriptGlobals globals = new ScriptGlobals();

        private readonly Engine engine = new Engine();

        private readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();

        public T Create<T>()
        {
            var type = typeof(T);

            lock (cache)
            {
                if (!cache.TryGetValue(type, out var obj))
                {
                    obj = Generate(type);
                    cache[type] = obj;
                }

                return (T)obj;
            }
        }

        private object Generate(Type type)
        {
            var references = new HashSet<Assembly>
            {
                typeof(Engine).Assembly,
                type.Assembly
            };
            var source = new StringBuilder();

            source.AppendLine($"internal sealed class {type.Name}_Impl : global::{type.FullName}");
            source.AppendLine("{");
            source.AppendLine("    private readonly global::WorkLibrary.Engine engine;");
            source.AppendLine();
            source.AppendLine($"    public {type.Name}_Impl(global::WorkLibrary.Engine engine)");
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

            source.Append($"Types.Add(typeof({type}), typeof({type.Name}_Impl));");

            var options = ScriptOptions.Default
                .WithOptimizationLevel(OptimizationLevel.Release)
                .WithReferences(references);
            var script = CSharpScript.Create(source.ToString(), options, typeof(ScriptGlobals));
            script.Compile();

            script.RunAsync(globals).Wait();

            return Activator.CreateInstance(globals.Types[type], engine);
        }
    }
}
