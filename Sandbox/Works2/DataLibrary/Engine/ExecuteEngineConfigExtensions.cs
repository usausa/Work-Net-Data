namespace DataLibrary.Engine
{
    public static class ExecuteEngineConfigExtensions
    {
        public static ExecuteEngine ToEngine(this ExecuteEngineConfig config) => new ExecuteEngine(config);
    }
}
