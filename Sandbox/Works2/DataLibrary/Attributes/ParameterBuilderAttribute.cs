﻿namespace DataLibrary.Attributes
{
    using System;
    using System.Data;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public abstract class ParameterBuilderAttribute : Attribute
    {
        public abstract Action<IDbDataParameter, object> CreateSetAction(Type type);
    }
}
