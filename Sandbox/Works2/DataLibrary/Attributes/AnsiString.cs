using System.Data;

namespace DataLibrary.Attributes
{
    using System;

    using Smart.Functional;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class AnsiString : ParameterAttribute
    {
        private readonly int? size;

        public AnsiString()
        {
        }

        public AnsiString(int size)
        {
            this.size = size;
        }

        public override Action<IDbDataParameter, object> CreateSetAction()
        {
            if (size.HasValue)
            {
                return (parameter, value) =>
                {
                    if (value is null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.DbType = DbType.AnsiStringFixedLength;
                        parameter.Size = size.Value;
                        parameter.Value = value;
                    }
                };
            }

            return (parameter, value) =>
            {
                if (value is null)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.DbType = DbType.AnsiString;
                    parameter.Value = value;
                }
            };
        }
    }
}
