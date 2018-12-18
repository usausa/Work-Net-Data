namespace WorkScript
{
    using System.Data;

    public interface IDao
    {
        int Count(IDbConnection con);
    }
}
