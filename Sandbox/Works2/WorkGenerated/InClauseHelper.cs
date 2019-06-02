using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace WorkGenerated
{
    public static class InClauseHelper
    {

        public static void AddParameter<T>(StringBuilder sql, DbCommand cmd, string name, DbType dbType, int size, T[] values)
        {
            // [MEMO] values.Length must > 0
            sql.Append("(");
            for (var i = 0; i < values.Length; i++)
            {
                var parameterName = String.Concat(name, "_", i.ToString(CultureInfo.InvariantCulture));

                sql.Append(parameterName);
                sql.Append(", ");

                DbCommandHelper.AddParameter(cmd, parameterName, dbType, size, values[i]);
            }

            sql.Length = sql.Length - 2;
            sql.Append(") ");
        }
    }
}
