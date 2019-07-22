using System;

namespace WorkPerformance
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using Microsoft.Data.Sqlite;

    using Smart.Data;
    using Smart.Data.Accessor.Engine;
    using Smart.Data.Accessor.Generator;
    using Smart.Data.Accessor.Loaders;

    public static class Program
    {
        public static void Main()
        {
            var config = new ExecuteEngineConfig();
            config.ConfigureComponents(components =>
            {
                components.Add<IDbProvider>(new DelegateDbProvider(() => new SqliteConnection(Connections.Memory)));
                var selector = new NamedDbProviderSelector();
                selector.AddProvider(string.Empty, new DelegateDbProvider(() => new SqliteConnection(Connections.Memory)));
                components.Add(selector);
            });

            var engine = config.ToEngine();

            var generator = new DaoGenerator(engine, new DummyLoader(MakeMiscDaoSql()), null);

            var watch = Stopwatch.StartNew();
            var dao1 = generator.Create<IFullSpecDao01>();
            Console.WriteLine(watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            var dao2 = generator.Create<IFullSpecDao02>();
            Console.WriteLine(watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            var dao3 = generator.Create<IFullSpecDao03>();
            Console.WriteLine(watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            var dao4 = generator.Create<IFullSpecDao04>();
            Console.WriteLine(watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();
            var dao5 = generator.Create<IFullSpecDao05>();
            Console.WriteLine(watch.ElapsedMilliseconds);

            Console.ReadLine();
        }

        private static Dictionary<string, string> MakeMiscDaoSql()
        {
            return new Dictionary<string, string>
            {
                {
                    "Execute",
                    "INSERT INTO Data (Id, Name) VALUES (@id, @name)"
                },
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
                    "/*% if (IsNotEmpty(code)) { */" +
                    "AND Code = /*@ code */ 'aaa'" +
                    "/*% } */"
                },
                {
                    "ExecuteIn1",
                    "SELECT * FROM Data" +
                    "/*% if (ids.Length > 0) { */" +
                    "WHERE Id IN /*@ ids */(1)" +
                    "/*% } */"
                },
                {
                    "ExecuteIn2",
                    "SELECT * FROM Data WHERE Id IN /*@ ids */(1)"
                },
                {
                    "ExecuteIn3",
                    "SELECT * FROM Data WHERE Id IN /*@ ids */(1)"
                },
                {
                    "QueryDataList",
                    "/*!helper WorkGenerator.Models.TestHelper */" +
                    "SELECT * FROM Data WHERE 1 = 1 " +
                    "/*% if (IsNotEmpty(name)) { */" +
                    "AND Name = /*@ name */ 'xxx'" +
                    "/*% } */" +
                    "/*% if (IsNotEmpty(code)) { */" +
                    "AND Code = /*@ code */ 'yyy'" +
                    "/*% } */" +
                    "ORDER BY /*# order */"
                },
                {
                    "QueryDataList2",
                    "/*!using System */" +
                    "SELECT * FROM Data WHERE 1 = 1 " +
                    "/*% if (!String.IsNullOrEmpty(parameter.Name)) { */" +
                    "AND Name = /*@ parameter.Name */ 'xxx'" +
                    "/*% } */" +
                    "/*% if (!String.IsNullOrEmpty(parameter.Code)) { */" +
                    "AND Code = /*@ parameter.Code */ 'yyy'" +
                    "/*% } */" +
                    "ORDER BY /*# parameter.Order */"
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
