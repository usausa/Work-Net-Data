namespace WorkResolver.Library
{
    public class DaoFactory
    {
        public DaoFactory(IExecutor executor, IConnectionManager connectionManager)
        {
        }

        public T Create<T>()
        {
            // TODO
            return default(T);
        }
    }
}
