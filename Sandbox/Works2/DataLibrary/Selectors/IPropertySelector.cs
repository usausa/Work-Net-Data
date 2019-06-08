namespace DataLibrary.Selectors
{
    using System.Reflection;

    public interface IPropertySelector
    {
        PropertyInfo SelectProperty(PropertyInfo[] properties, string name);
    }
}
