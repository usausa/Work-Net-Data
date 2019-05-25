namespace DataLibrary.Providers
{
    using System.Data.Common;

    public interface IDbProvider
    {
        DbConnection CreateConnection();

        DbParameter CreateParameter();
    }
}
