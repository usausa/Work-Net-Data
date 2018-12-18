namespace WorkResolver.Library
{
    using System.Data;

    public interface IExecutor
    {
        void Execute(IDbConnection con);
    }
}
