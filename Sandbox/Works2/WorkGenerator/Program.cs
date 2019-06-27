using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
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
            // TODO config by mock
            var config = new ExecuteEngineConfig();
            config.Components.Add<IDbProvider>(new DelegateDbProvider(() => new SqliteConnection(Connections.Memory)));

            var engine = config.ToEngine();

            // TODO Loader
            var generator = new DaoGenerator(null, engine, new Debugger());

            //var dao = generator.Create<ISimpleExecuteDao>();
            var dao2 = generator.Create<IFullSpecDao>();

            // TODO use
        }
    }

    public class Debugger : IGeneratorDebugger
    {
        public void Log(bool success, DaoSource source, BuildError[] errors)
        {
            Debug.WriteLine("================================================================================");
            Debug.WriteLine($"TargetType=[{source.TargetType}] : Result=[{success}]");
            Debug.WriteLine("================================================================================");
            foreach (var reference in source.References)
            {
                Debug.WriteLine(reference.FullName);
            }
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
