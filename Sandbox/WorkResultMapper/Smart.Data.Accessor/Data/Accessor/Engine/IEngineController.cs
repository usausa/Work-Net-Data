namespace Smart.Data.Accessor.Engine
{
    public interface IEngineController
    {
        DiagnosticsInfo Diagnostics { get; }

        void ClearResultMapperCache();

        void ClearDynamicSetupCache();
    }
}
