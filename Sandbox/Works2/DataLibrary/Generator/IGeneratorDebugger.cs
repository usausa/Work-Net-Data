namespace DataLibrary.Generator
{
    using System;

    public interface IGeneratorDebugger
    {
        void Log(Type type, string source);
    }
}
