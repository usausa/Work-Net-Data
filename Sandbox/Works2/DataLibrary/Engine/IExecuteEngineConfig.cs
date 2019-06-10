namespace DataLibrary.Engine
{
    using Smart.ComponentModel;

    public interface IExecuteEngineConfig
    {
        IComponentContainer CreateComponentContainer();
    }
}
