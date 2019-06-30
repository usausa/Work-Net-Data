using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using DataLibrary.Loader;
using DataLibrary.Providers;
using Microsoft.Data.Sqlite;

namespace WorkGenerator
{
    using DataLibrary.Engine;
    using DataLibrary.Generator;

    using WorkGenerator.Dao;

    public static class Program
    {
        public static void Main()
        {
            //ListReference(new HashSet<Assembly>(), typeof(ExecuteEngine).Assembly);

            // TODO config by mock
            var config = new ExecuteEngineConfig();
            config.Components.Add<IDbProvider>(new DelegateDbProvider(() => new SqliteConnection(Connections.Memory)));
            var selector = new NamedDbProviderSelector();
            selector.AddProvider(string.Empty, new DelegateDbProvider(() => new SqliteConnection(Connections.Memory)));
            config.Components.Add(selector);

            var engine = config.ToEngine();

            // TODO Loader
            var generator = new DaoGenerator(new DummyLoader(), engine, new Debugger());

            //var dao = generator.Create<ISimpleExecuteDao>();
            var dao2 = generator.Create<IFullSpecDao>();

            // TODO use
        }

        //private static void ListReference(HashSet<Assembly> cached, Assembly assembly)
        //{
        //    if (cached.Contains(assembly))
        //    {
        //        return;
        //    }

        //    cached.Add(assembly);
        //    Debug.WriteLine("::: " + assembly.FullName);

        //    foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
        //    {
        //        ListReference(cached, Assembly.Load(referencedAssembly));
        //    }
        //}
    }

    public class DummyLoader : ISqlLoader
    {
        public string Load(MethodInfo mi) => "";
    }

    public class Debugger : IGeneratorDebugger
    {
        public void Log(bool success, DaoSource source, BuildError[] errors)
        {
            Debug.WriteLine("================================================================================");
            Debug.WriteLine($"TargetType=[{source.TargetType}] : Result=[{success}]");
            Debug.WriteLine("================================================================================");
            Debug.Write(source.Code);
            Debug.WriteLine("================================================================================");
            if (!success)
            {
                foreach (var error in errors)
                {
                    Debug.WriteLine($"[{error.Id}] ({error.Start}...{error.End}) {error.Message}");
                    Debug.WriteLine(source.Code.Substring(error.Start, error.End - error.Start));
                }
                Debug.WriteLine("================================================================================");
            }
        }
    }
}
