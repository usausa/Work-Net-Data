namespace DataWork
{
    using DataLibrary.Attributes;

    public static class Program
    {
        public static void Main(string[] args)
        {
        }
    }

    [Dao]
    public interface ISampleDao
    {
        // test pattern
        // TODO pattern 3
        // TODO sp return void
        // TODO ref, out ?
        // TODO Task
        // TODO with con, con+tx, tx-only ?
        // TODO default sql search
        // TODO parameter 1, n ?

        //[Execute]
        int Execute(int id);
    }

    // TODO
    public sealed class SampleDaoImpl : ISampleDao
    {
        public int Execute(int id)
        {
            return 0;
            //return engine.Execute();
        }
    }
}
