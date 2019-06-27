using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

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
            var engine = new ExecuteEngineConfig()
                .ToEngine();

            // TODO Loader
            var generator = new DaoGenerator(null, engine, new Debugger());

            var dao = generator.Create<ISimpleExecuteDao>();
            //var dao2 = generator.Create<IFullSpecDao>();

            // TODO use
        }
    }

    public class Debugger : IGeneratorDebugger
    {
        public void Log(ClassMetadata metadata, string source, Assembly[] references)
        {
            Debug.WriteLine("================================================================================");
            foreach (var reference in references)
            {
                Debug.WriteLine(reference.FullName);
            }
            Debug.WriteLine("================================================================================");
            Debug.Write(source);
            Debug.WriteLine("================================================================================");
        }
    }
}
