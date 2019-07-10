namespace WorkDynamic
{
    using System.Diagnostics;
    using System.Reflection;

    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Engine;
    using Smart.Data.Accessor.Generator;
    using Smart.Data.Accessor.Loaders;

    public static class Program
    {
        public static void Main()
        {
            var engine = new ExecuteEngineConfig()
                .ToEngine();

            var generator = new DaoGenerator(engine, new DummyLoader(), new Debugger());

            var dao = generator.Create<IDynamicDao>();
        }
    }

    public class DummyLoader : ISqlLoader
    {
        public string Load(MethodInfo mi)
        {
            return "SELECT COUNT * FROM Data " +
                   "WHERE 1 = 1 " +
                   "/*% foreach (var data in entries) { */" +
                   "OR (Key1 = /*@ data.Key1 */ AND Key2 = /*@ data.Key2 */)" +
                   "/*% } */";
        }
    }

    [Dao]
    public interface IDynamicDao
    {
        [ExecuteScalar]
        long Count(Data[] entries);
    }

    public class Data
    {
        public int Kye1 { get; set; }

        public string Kye2 { get; set; }
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
