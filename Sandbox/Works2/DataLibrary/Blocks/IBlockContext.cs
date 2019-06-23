namespace DataLibrary.Blocks
{
    public interface IBlockContext
    {
        void AddSql(string sql);

        void AddCode(string code);

        void AddParameter(string parameter);
    }
}
