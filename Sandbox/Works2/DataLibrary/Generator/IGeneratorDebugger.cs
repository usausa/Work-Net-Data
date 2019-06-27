namespace DataLibrary.Generator
{
    public interface IGeneratorDebugger
    {
        void Log(bool success, DaoSource source, BuildError[] errors);
    }
}
