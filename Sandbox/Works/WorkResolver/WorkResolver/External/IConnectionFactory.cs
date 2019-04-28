namespace WorkResolver.External
{
    using System.Data;

    public interface IConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
