namespace WorkGenerated
{
    using System.Diagnostics;
    using System.Reflection;

    using Smart.Data;
    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Engine;
    using Smart.Data.Accessor.Generator;
    using Smart.Data.Accessor.Loaders;
    using Smart.Mock.Data;

    public static class Program
    {
        public static void Main()
        {
            var mockExecute = new MockDbConnection();
            mockExecute.SetupCommand(cmd => cmd.SetupResult(1));

            var provider = new DelegateDbProvider(() => mockExecute);

            var engine = new ExecuteEngineConfig()
                .ConfigureComponents(c =>
                {
                    c.Add<IDbProvider>(provider);
                })
                .ToEngine();

            var generator = new DaoGenerator(engine, new DummyLoader(), new Debugger());

            var dao = generator.Create<IBenchmarkDao>();
        }
    }

    public class DummyLoader : ISqlLoader
    {
        public string Load(MethodInfo mi)
        {
            return "INSERT INTO Table (Id, Name) VALUES (/*@ entity.Id */1, /*@ entity.Name */'xxx')";
        }
    }

    [Dao]
    public interface IBenchmarkDao
    {
        [Execute]
        int Execute(DataEntity entity);
    }

    public class DataEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public sealed class Debugger : IGeneratorDebugger
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
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
