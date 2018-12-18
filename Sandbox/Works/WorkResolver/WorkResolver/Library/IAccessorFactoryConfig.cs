namespace WorkResolver.Library
{
    public interface IAccessorFactoryConfig
    {
        IExecutor Executor { get; }

        IConnectionManager ConnectionManager { get; }
    }
}
