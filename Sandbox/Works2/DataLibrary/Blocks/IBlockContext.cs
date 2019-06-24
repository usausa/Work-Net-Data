namespace DataLibrary.Blocks
{
    public interface IBlockContext
    {
        void AddImport(string ns, bool isStatic);

        void AddSql(string sql);

        void AddRawSql(string sql);

        void AddCode(string code);

        void AddParameter(string parameter);
    }
}
