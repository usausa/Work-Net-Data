namespace DataLibrary.Results
{
    using System;

    public interface IResultParser
    {
        object Parse(Type destinationType, object value);
    }
}
