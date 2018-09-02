namespace WorkScript
{
    using System;
    using System.Data;

    using Dapper;

    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.Data.Sqlite;

    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();

                con.Execute("CREATE TABLE TEST(ID INTEGER, DATA TEXT, PRIMARY KEY (ID))");

                var options = ScriptOptions.Default
                    .WithReferences(
                        typeof(IDao).Assembly,
                        typeof(IDbConnection).Assembly,
                        typeof(SqlMapper).Assembly);
                var script = CSharpScript.Create(
                    "using WorkScript;" +
                    "using System.Data;" +
                    "using Dapper;" +
                    "public sealed class Dao : IDao { public int Count(IDbConnection con) => con.Execute(\"SELECT COUNT(*) FROM TEST\"); }" +
                    "Types.Add(typeof(Dao).Name, typeof(Dao));",
                    options,
                    typeof(ScriptGlobals));
                script.Compile();

                var globals = new ScriptGlobals();
                script.RunAsync(globals).Wait();

                var dao = (IDao)Activator.CreateInstance(globals.Types["Dao"]);

                var count = dao.Count(con);
                Console.WriteLine(count);
            }
        }
    }
}
