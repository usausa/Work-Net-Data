namespace DataLibrary.Helpers
{
    using System.Data.Common;
    using System.Reflection;
    using System.Threading;

    using DataLibrary.Attributes;

    internal static class ParameterHelper
    {
        public static bool IsTimeoutParameter(ParameterInfo pi) =>
            pi.GetCustomAttribute<TimeoutParameterAttribute>() != null;

        public static bool IsCancellationTokenParameter(ParameterInfo pi) =>
            pi.ParameterType == typeof(CancellationToken);

        public static bool IsConnectionParameter(ParameterInfo pi) =>
            typeof(DbConnection).IsAssignableFrom(pi.ParameterType);

        public static bool IsTransactionParameter(ParameterInfo pi) =>
            typeof(DbTransaction).IsAssignableFrom(pi.ParameterType);

        public static bool IsSqlParameter(ParameterInfo pi) =>
            !IsTimeoutParameter(pi) &&
            !IsCancellationTokenParameter(pi) &&
            !IsConnectionParameter(pi) &&
            !IsTransactionParameter(pi);
    }
}
