namespace WorkProcedure
{
    using System.Diagnostics;

    using Microsoft.Data.SqlClient;

    using Smart.Data;
    using Smart.Data.Accessor;
    using Smart.Data.Accessor.Attributes;
    using Smart.Data.Accessor.Engine;
    using Smart.Data.Accessor.Generator;

    public static class Program
    {
        public static void Main()
        {
            var config = new ExecuteEngineConfig();
            config.ConfigureComponents(components =>
            {
                components.Add<IDbProvider>(new DelegateDbProvider(() => new SqlConnection("Data Source=SERVER;Initial Catalog=Test;User ID=test;Password=test")));
            });

            var engine = config.ToEngine();
            var generator = new DaoGenerator(engine, null, new Debugger());

            var dao = generator.Create<IProcedureDao>();

            var param = new Parameter { Param1 = 1, Param2 = 2 };
            var ret = dao.Execute(param);
            var scalar = dao.ExecuteScalar(param);
            var entity = dao.ExecuteQuery(param);
        }
    }

    public class Entity
    {
        public int Id { get; set; }

        public string Data { get; set; }
    }

    [Dao]
    public interface IProcedureDao
    {
        [Procedure("TestExecute")]
        int Execute(Parameter parameter);

        [Procedure("TestExecuteScalar", MethodType.ExecuteScalar)]
        int ExecuteScalar(Parameter parameter);

        [Procedure("TestQuery", MethodType.QueryFirstOrDefault)]
        Entity ExecuteQuery(Parameter parameter);
    }

    public class Parameter
    {
        [Input]
        [Name("param1")]
        public int Param1 { get; set; }

        [InputOutput]
        [Name("param2")]
        public int Param2 { get; set; }

        [Output]
        [Name("param3")]
        public int Param3 { get; set; }

        [ReturnValue]
        public int RetunValue { get; set; }
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
