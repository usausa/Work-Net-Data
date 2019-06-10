namespace DataLibrary.Engine
{
    using DataLibrary.Selectors;

    using Smart.ComponentModel;
    using Smart.Converter;
    using Smart.Reflection;

    public sealed class ExecuteEngineConfig : IExecuteEngineConfig
    {
        public ComponentConfig Components { get; } = new ComponentConfig();

        public ExecuteEngineConfig()
        {
            Components.Add<IDelegateFactory>(DelegateFactory.Default);
            Components.Add<IObjectConverter>(ObjectConverter.Default);
            Components.Add<IPropertySelector>(DefaultPropertySelector.Instance);
        }

        public IComponentContainer CreateComponentContainer()
        {
            return Components.ToContainer();
        }
    }
}
