using DataLibrary.Attributes;

namespace WorkGenerator.Dao
{
    [Dao]
    public interface ISimpleExecuteDao
    {

        int Execute();
    }
}
