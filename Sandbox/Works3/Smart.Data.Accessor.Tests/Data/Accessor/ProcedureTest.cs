namespace Smart.Data.Accessor
{
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using Smart.Data.Accessor.Attributes;
    using Smart.Mock;
    using Smart.Mock.Data;

    using Xunit;

    public class ProcedureTest
    {
        //--------------------------------------------------------------------------------
        // Class Parameter
        //--------------------------------------------------------------------------------

        public class Parameter
        {
            [Input]
            public string InParam { get; set; }

            [InputOutput]
            public int? InOutParam { get; set; }

            [Output]
            public int OutParam { get; set; }

            [ReturnValue]
            public int Result { get; set; }
        }

        [Dao]
        public interface IProcedureDao
        {
            [Procedure("PROC", false)]
            void Call(DbConnection con, Parameter parameter);
        }

        [Fact]
        public void CallByClassParameter()
        {
            using (TestDatabase.Initialize())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .Build();

                var dao = generator.Create<IProcedureDao>();

                var cmd = new MockDbCommand
                {
                    Executing = c =>
                    {
                        Assert.Equal("PROC", c.CommandText);
                        Assert.Equal("1", c.Parameters[nameof(Parameter.InParam)].Value);
                        Assert.Equal(2, c.Parameters[nameof(Parameter.InOutParam)].Value);

                        c.Parameters[nameof(Parameter.InOutParam)].Value = 3;
                        c.Parameters[nameof(Parameter.OutParam)].Value = 4;
                        c.Parameters.OfType<MockDbParameter>().First(x => x.Direction == ParameterDirection.ReturnValue).Value = 5;
                    }
                };
                cmd.SetupResult(100);
                var con = new MockDbConnection();
                con.SetupCommand(cmd);

                var parameter = new Parameter
                {
                    InParam = "1",
                    InOutParam = 2
                };
                dao.Call(con, parameter);

                Assert.Equal(3, parameter.InOutParam);
                Assert.Equal(4, parameter.OutParam);
                Assert.Equal(5, parameter.Result);
            }
        }

        //--------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------

        [Dao]
        public interface IProcedure2Dao
        {
            [Procedure("PROC")]
            int Call(DbConnection con, int param1, ref int param2, out int param3);
        }

        [Fact]
        public void Call()
        {
            using (TestDatabase.Initialize()
                .SetupDataTable())
            {
                var generator = new GeneratorBuilder()
                    .EnableDebug()
                    .Build();

                var dao = generator.Create<IProcedure2Dao>();

                var cmd = new MockDbCommand
                {
                    Executing = c =>
                    {
                        Assert.Equal("PROC", c.CommandText);
                        Assert.Equal(1, c.Parameters["param1"].Value);
                        Assert.Equal(2, c.Parameters["param2"].Value);

                        c.Parameters["param2"].Value = 3;
                        c.Parameters["param3"].Value = 4;
                        c.Parameters.OfType<MockDbParameter>().First(x => x.Direction == ParameterDirection.ReturnValue).Value = 5;
                    }
                };
                cmd.SetupResult(100);
                var con = new MockDbConnection();
                con.SetupCommand(cmd);

                var param2 = 2;
                var ret = dao.Call(con, 1, ref param2, out var param3);

                Assert.Equal(3, param2);
                Assert.Equal(4, param3);
                Assert.Equal(5, ret);
            }
        }
    }
}
