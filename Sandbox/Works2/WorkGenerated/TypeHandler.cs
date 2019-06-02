﻿namespace WorkGenerated
{
    using System;
    using System.Data;

    public interface ITypeHandler
    {
        void SetValue(IDbDataParameter parameter, object value);

        object Parse(Type destinationType, object value);
    }
}