using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace WorkGenerated
{
    public static class DaoHelper
    {

    }


    public class SampleDao
    {
        private readonly IDbProvider provider;

        private readonly Func<object, object> converter;

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        public int Execute()
        {
            // Connection
            using (var con = provider.CreateConnection())
            {
                // Command
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                    // Execute
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // With Cancel
        public async Task<int> ExecuteAsync(CancellationToken cancel)
        {
            // Connection
            using (var con = (DbConnection)provider.CreateConnection())
            {
                // Command
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                    // Execute
                    await con.OpenAsync(cancel).ConfigureAwait(false);

                    return await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
                }
            }
        }

        // With Connection
        public int Execute(IDbConnection con)
        {
            // Command
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Data (Id, Name) VALUES (1, 'test')";

                // Execute
                return cmd.ExecuteNonQuery();
            }
        }

        // TODO

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------


    }
}
