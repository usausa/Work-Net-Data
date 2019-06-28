namespace DataLibrary.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Threading;

    using DataLibrary.Attributes;
    using DataLibrary.Blocks;

    internal sealed class MethodMetadata
    {
        public int No { get; }

        public MethodInfo MethodInfo { get; }

        public CommandType CommandType { get; }

        public MethodType MethodType { get; }

        public IReadOnlyList<IBlock> Blocks { get; }

        public bool IsAsync { get; }

        public Type EngineResultType { get; }

        // Method attribute

        public ProviderAttribute Provider { get; }

        public TimeoutAttribute Timeout { get; }

        // Parameter

        public ParameterInfo TimeoutParameter { get; }

        public ParameterInfo CancelParameter { get; }

        public ParameterInfo ConnectionParameter { get; }

        public ParameterInfo TransactionParameter { get; }

        public MethodMetadata(int no, MethodInfo mi, CommandType commandType, MethodType memberType, IReadOnlyList<IBlock> blocks)
        {
            No = no;
            MethodInfo = mi;
            CommandType = commandType;
            MethodType = memberType;
            Blocks = blocks;

            IsAsync = mi.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            EngineResultType = IsAsync
                ? (mi.ReturnType.IsGenericType
                    ? mi.ReturnType.GetGenericArguments()[0]
                    : typeof(void))
                : mi.ReturnType;

            Provider = mi.GetCustomAttribute<ProviderAttribute>();
            Timeout = mi.GetCustomAttribute<TimeoutAttribute>();

            foreach (var pi in mi.GetParameters())
            {
                if (pi.GetCustomAttribute<TimeoutParameterAttribute>() != null)
                {
                    if ((pi.ParameterType != typeof(int)) && (pi.ParameterType != typeof(int?)))
                    {
                        throw new AccessorException($"Timeout parameter type must be int. type=[{mi.DeclaringType.FullName}], method=[{mi.Name}], parameter=[{pi.Name}]");
                    }

                    TimeoutParameter = pi;
                }

                if (pi.ParameterType == typeof(CancellationToken))
                {
                    CancelParameter = pi;
                }

                if (typeof(DbConnection).IsAssignableFrom(pi.ParameterType))
                {
                    ConnectionParameter = pi;
                }

                if (typeof(DbTransaction).IsAssignableFrom(pi.ParameterType))
                {
                    TransactionParameter = pi;
                }
            }
        }
    }
}
