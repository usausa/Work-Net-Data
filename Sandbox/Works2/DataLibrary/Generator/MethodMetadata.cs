using DataLibrary.Helpers;

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
    using DataLibrary.Nodes;

    internal sealed class MethodMetadata
    {
        public int No { get; }

        public MethodInfo MethodInfo { get; }

        public CommandType CommandType { get; }

        public MethodType MethodType { get; }

        public IReadOnlyList<INode> Nodes { get; }

        public IReadOnlyList<ParameterEntry> Parameters { get; }

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

        // Helper

        public bool HasConnectionParameter => ConnectionParameter != null || TransactionParameter != null;

        public MethodMetadata(
            int no,
            MethodInfo mi,
            CommandType commandType,
            MethodType memberType,
            IReadOnlyList<INode> nodes,
            IReadOnlyList<ParameterEntry> parameters)
        {
            No = no;
            MethodInfo = mi;
            CommandType = commandType;
            MethodType = memberType;
            Nodes = nodes;
            Parameters = parameters;

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
                if (ParameterHelper.IsTimeoutParameter(pi))
                {
                    TimeoutParameter = pi;
                }

                if (ParameterHelper.IsCancellationTokenParameter(pi))
                {
                    CancelParameter = pi;
                }

                if (ParameterHelper.IsConnectionParameter(pi))
                {
                    ConnectionParameter = pi;
                }

                if (ParameterHelper.IsTransactionParameter(pi))
                {
                    TransactionParameter = pi;
                }
            }
        }
    }
}
