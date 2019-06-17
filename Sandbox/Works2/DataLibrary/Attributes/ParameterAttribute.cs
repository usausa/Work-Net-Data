namespace DataLibrary.Attributes
{
    using System;
    using System.Data;

    public abstract class ParameterAttribute : Attribute
    {
        public abstract Action<IDbDataParameter, object> CreateSetAction(Type type);
    }
}
