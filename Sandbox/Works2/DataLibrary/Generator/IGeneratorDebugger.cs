namespace DataLibrary.Generator
{
    using System.Reflection;

    public interface IGeneratorDebugger
    {
        void Log(ClassMetadata metadata, string source, Assembly[] references);
    }
}
