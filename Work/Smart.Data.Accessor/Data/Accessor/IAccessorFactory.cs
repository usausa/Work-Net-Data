namespace Smart.Data.Accessor
{
    using System;

    public interface IAccessorFactory
    {
        T Create<T>();

        object Create(Type type);
    }
}
