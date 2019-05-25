using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DataLibrary.Providers;

namespace DataWork
{
    using System.Data;
    using System.Text;

    using DataLibrary.Engine;

    using Smart.Data;


    public static class Program
    {
        public static void Main()
        {
        }
    }

    // TODO
    public sealed class SampleDaoImpl
    {
        private readonly IDbProvider provider;

        private readonly Func<object, object> converter;

        //--------------------------------------------------------------------------------
        // Basic
        //--------------------------------------------------------------------------------

        public int Execute()
        {
            using (var con = provider.CreateConnection())
            {
                return ExecuteEngine.Execute(
                    con,
                    null,
                    "xxx",
                    CommandType.Text,
                    null,
                    null);
            }
        }

        public Task<int> ExecuteAsync()
        {
            using (var con = provider.CreateConnection())
            {
                return ExecuteEngine.ExecuteAsync(
                    con,
                    null,
                    "xxx",
                    CommandType.Text,
                    null,
                    null,
                    default);
            }
        }

        public long ExecuteScalar()
        {
            using (var con = provider.CreateConnection())
            {
                return ExecuteEngine.ExecuteScalar<long>(
                    con,
                    null,
                    "xxx",
                    CommandType.Text,
                    null,
                    null,
                    converter);
            }
        }

        public Task<long> ExecuteScalarAsync()
        {
            using (var con = provider.CreateConnection())
            {
                return ExecuteEngine.ExecuteScalarAsync<long>(
                    con,
                    null,
                    "xxx",
                    CommandType.Text,
                    null,
                    null,
                    converter,
                    default);
            }
        }

        // TODO reader, query, queryFoD 2

        //--------------------------------------------------------------------------------
        // Extra
        //--------------------------------------------------------------------------------

        // TODO Procedure(*), Insert(*)

        //--------------------------------------------------------------------------------
        // Connection
        //--------------------------------------------------------------------------------

        // TODO parameter

        // TODO parameter with tx

        // TODO named connection

        //--------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------

        // TODO out

        // TODO class with direction

        // TODO dbType and Size with parameter

        // TODO dbType and Size with class

        // TODO TypeHandler with parameter

        // TODO TypeHandler with class

        //--------------------------------------------------------------------------------
        // Option
        //--------------------------------------------------------------------------------

        // TODO Timeout by attribute

        // TODO Timeout by parameter

        // TODO Buffer by attribute

        // TODO Buffer by parameter

        // TODO cancel

        // TODO Reader option

        //--------------------------------------------------------------------------------
        // Condition
        //--------------------------------------------------------------------------------

        public int ParameterOptimized(int id)
        {
            var parameters = new List<DbParameter>();
            //parameters.Add(); TODO

            using (var con = provider.CreateConnection())
            {
                return ExecuteEngine.Execute(
                    con,
                    null,
                    "SELECT * FROM Test WHERE Id = @id",
                    CommandType.Text,
                    null,
                    null);
            }
        }

        public int ParameterConditional(int? id)
        {
            var parameters = new List<DbParameter>();

            var sql = new StringBuilder(32);
            sql.Append("SELECT * FROM Test");
            if (id != null)
            {
                sql.Append(" WHERE Id = @id");
                //parameters.Add(); TODO
            }

            using (var con = provider.CreateConnection())
            {
                return ExecuteEngine.Execute(
                    con,
                    null,
                    sql.ToString(),
                    CommandType.Text,
                    null,
                    null);
            }
        }

        // TODO parameter 2

        // TODO parameter class
    }

    //public class ParameterBinder
    //{
    //    public void BindObject<T>(IDbCommand cmd, T value) where T : class
    //    {
    //        var parameter = cmd.CreateParameter();
    //        if (value is null)
    //        {
    //            parameter.Value = DBNull.Value;
    //        }
    //        else
    //        {

    //        }
    //        parameter.Value = value is null ? DBNull.Value : value;
    //    }

    //    public void BindValue<T>(IDbCommand cmd, T value) where T : struct
    //    {
    //        var parameter = cmd.CreateParameter();
    //        parameter.Value = value;
    //    }
    //}
}
