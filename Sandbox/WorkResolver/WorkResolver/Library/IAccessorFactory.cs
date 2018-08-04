namespace WorkResolver.Library
{
    using System;

    public interface IAccessorFactory
    {
        T Create<T>();

        object Create(Type type);
    }
}
