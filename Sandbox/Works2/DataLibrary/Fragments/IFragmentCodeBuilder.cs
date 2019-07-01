namespace DataLibrary.Fragments
{
    public interface IFragmentCodeBuilder
    {
        void AddHelper(string value);

        void AddSql(string value);

        void AddRawSql(string value);

        void AddCode(string value);

        void AddParameter(string value);
    }
}
