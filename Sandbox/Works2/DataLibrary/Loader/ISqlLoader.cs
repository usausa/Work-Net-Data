namespace DataLibrary.Loader
{
    using System.Reflection;

    public interface ISqlLoader
    {
        string Load(MethodInfo mi);
    }
}
