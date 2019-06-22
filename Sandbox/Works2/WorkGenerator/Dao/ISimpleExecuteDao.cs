namespace WorkGenerator.Dao
{
    using DataLibrary.Attributes;

    [Dao]
    public interface ISimpleExecuteDao
    {
        [Execute]

        int Execute();
    }
}
