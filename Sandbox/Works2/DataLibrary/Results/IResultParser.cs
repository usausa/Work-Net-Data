namespace DataLibrary.Results
{
    using System;

    public interface IResultParser
    {
        object Parse(Type type, object value);
    }
}
