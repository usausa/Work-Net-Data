namespace DataWork
{
    using DataLibrary.Attributes;

    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    [Dao]
    public interface ISampleDao
    {
        // TODO pattern 3
        // TODO sp return void
        // TODO ref, out ?
        // TODO Task
        // TODO with con, con+tx, tx-only ?
        // TODO default sql search

        [Execute(nameof(Execute))]
        int Execute(int id);
    }
}
