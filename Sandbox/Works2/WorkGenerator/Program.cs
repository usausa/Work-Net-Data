using System.Collections.Generic;
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

            var config = new ExecuteEngineConfig();
            config.Components.Add<IDbProvider>(new DelegateDbProvider(() => new SqliteConnection(Connections.Memory)));
            var selector = new NamedDbProviderSelector();
            selector.AddProvider(string.Empty, new DelegateDbProvider(() => new SqliteConnection(Connections.Memory)));
            config.Components.Add(selector);

            var engine = config.ToEngine();

            var generator = new DaoGenerator(new DummyLoader(MakeMiscDaoSql()), engine, new Debugger());

            //var dao = generator.Create<ISimpleExecuteDao>();
            //var dao2 = generator.Create<IFullSpecDao>();
            var dao3 = generator.Create<IMiscDao>();

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

        private static Dictionary<string, string> MakeMiscDaoSql()
        {
            return new Dictionary<string, string>
            {
                {
                    "Count",
                    "SELECT COUNT(*) FROM Data"
                },
                {
                    "Count1",
                    "SELECT COUNT(*) FROM Data WHERE Name = /*@ name */ 'xxx' AND Code = /*@ code */ 'aaa'"
                },
                {
                    "Count2",
                    "/*!helper WorkGenerator.Models.TestHelper */" +
                    "SELECT COUNT(*) FROM Data WHERE 1 = 1" +
                    "/*% if (IsNotEmpty(code) { */" +
                    "AND Code = /*@ code */ 'aaa'" +
                    "/*% } */"
                },
                {
                    "QueryDataList",
                    "/*!helper WorkGenerator.Models.TestHelper */" +
                    "SELECT * FROM Data WHERE 1 = 1 " +
                    "/*% if (IsNotEmpty(name) { */" +
                    "AND Name = /*@ name */ 'xxx'" +
                    "/*% } */" +
                    "/*% if (IsNotEmpty(code) { */" +
                    "AND Code = /*@ code */ 'yyy'" +
                    "/*% } */" +
                    "ORDER BY /*# order */"
                },
                {
                    "QueryDataList2",
                    "/*!using System */" +
                    "SELECT * FROM Data WHERE 1 = 1 " +
                    "/*% if (!String.IsNullOrEmtpy(parameter.Name) { */" +
                    "AND Name = /*@ parameter.Name */ 'xxx'" +
                    "/*% } */" +
                    "/*% if (!String.IsNullOrEmtpy(parameter.Code) { */" +
                    "AND Code = /*@ parameter.Code */ 'yyy'" +
                    "/*% } */" +
                    "ORDER BY /*# parameter.Order */"
                },
                {
                    "ExecuteIn1",
                    "/*@ ids */"
                },
                {
                    "ExecuteIn2",
                    "/*@ ids */"
                },
                {
                    "ExecuteIn3",
                    "/*@ ids */"
                }
            };
        }
    }

    public class DummyLoader : ISqlLoader
    {
        private readonly Dictionary<string, string> values;

        public DummyLoader()
            : this(new Dictionary<string, string>())
        {
        }

        public DummyLoader(Dictionary<string, string> values)
        {
            this.values = values;
        }

        public string Load(MethodInfo mi) => values.TryGetValue(mi.Name, out var sql) ? sql : string.Empty;
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
