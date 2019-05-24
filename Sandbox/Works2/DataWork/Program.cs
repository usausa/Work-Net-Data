using DataLibrary.Engine;

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
        // test pattern
        // TODO pattern 3
        // TODO sp return void
        // TODO ref, out ?
        // TODO Task
        // TODO with con, con+tx, tx-only ?
        // TODO default sql search
        // TODO parameter 1, n ?

        [Execute]
        int Execute(int id);
    }

    // TODO
    public sealed class ISampleDaoImpl : ISampleDao
    {
        private readonly ExecuteEngine engine;

        public ISampleDaoImpl(ExecuteEngine engine)
        {
            this.engine = engine;
        }

        public int Execute(int id)
        {
            return 0;
            //return engine.Execute();
        }
    }
}
