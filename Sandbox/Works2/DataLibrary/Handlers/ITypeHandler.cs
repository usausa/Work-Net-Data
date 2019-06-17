namespace DataLibrary.Handlers
{
    using System;
    using System.Data;

    public interface ITypeHandler
    {
        void SetValue<T>(IDbDataParameter parameter, T value);

        object Parse(Type destinationType, object value);
    }
}
