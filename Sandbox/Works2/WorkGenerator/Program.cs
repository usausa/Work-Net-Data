using System;
using System.Diagnostics;

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
            // TODO use
        }
    }

    public class Debugger : IGeneratorDebugger
    {
        public void Log(Type type, string source)
        {
            Debug.WriteLine("================================================================================");
            Debug.WriteLine(source);
            Debug.WriteLine("================================================================================");
        }
    }
}
