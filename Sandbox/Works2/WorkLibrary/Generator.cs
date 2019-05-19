using System;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace WorkLibrary
{
    public class Generator
    {
        private readonly ScriptGlobals globals = new ScriptGlobals();

        private readonly Engine engine = new Engine();

        // TODO 1: cache, 2: alt namespace same name ?

        public T Create<T>()
        {
            // TODO create by interface, method, ret, parameter  same mehod
            var options = ScriptOptions.Default;
            //options.AddReferences(option.Assemblies);
            options.AddReferences(typeof(Engine).Assembly);
            options.AddReferences(typeof(T).Assembly);
            //options.AddImports(option.Imports);
            options.AddImports(typeof(Engine).Namespace);
            options.AddImports(typeof(T).Namespace);

            var source = new StringBuilder();
            // TODO
            // namespace ?
            source.Append($"Types.Add(typeof({typeof(T)}), typeof({typeof(T)}_Impl));");

            var script = CSharpScript.Create(source.ToString(), options, typeof(ScriptGlobals));
            script.Compile();

            script.RunAsync(globals).Wait();

            return (T)Activator.CreateInstance(globals.Types[typeof(T)], engine);
        }
    }
}
