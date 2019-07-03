﻿namespace DataLibrary.Attributes
{
    using System;
    using System.Data;

    public sealed class AnsiString : ParameterBuilderAttribute
    {
        private readonly int? size;

        public AnsiString()
        {
        }

        public AnsiString(int size)
        {
            this.size = size;
        }

        public override Action<IDbDataParameter, object> CreateSetAction(Type type)
        {
            if (size.HasValue)
            {
                return (parameter, value) =>
                {
                    parameter.DbType = DbType.AnsiStringFixedLength;
                    parameter.Size = size.Value;
                    parameter.Value = value;
                };
            }

            return (parameter, value) =>
            {
                parameter.DbType = DbType.AnsiString;
                parameter.Value = value;
            };
        }
    }
}
