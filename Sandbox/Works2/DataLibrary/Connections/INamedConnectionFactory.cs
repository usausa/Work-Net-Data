namespace DataLibrary.Connections
{
    using System.Data;

    public interface INamedConnectionFactory
    {
        IDbConnection CreateConnection(string name);
    }
}
