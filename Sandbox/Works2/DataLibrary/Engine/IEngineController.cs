namespace DataLibrary.Engine
{
    public interface IEngineController
    {
        int CountResultMapperCache { get; }

        void ClearResultMapperCache();
    }
}
