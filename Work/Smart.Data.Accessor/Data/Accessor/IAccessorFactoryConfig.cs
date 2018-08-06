namespace Smart.Data.Accessor
{
    using Smart.Data.Accessor.Connection;
    using Smart.Data.Accessor.Executor;

    public interface IAccessorFactoryConfig
    {
        IExecutor Executor { get; }

        IConnectionManager ConnectionManager { get; }
    }
}
