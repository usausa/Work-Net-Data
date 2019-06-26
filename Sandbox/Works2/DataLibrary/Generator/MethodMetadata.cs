namespace DataLibrary.Generator
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;

    using DataLibrary.Attributes;

    public sealed class MethodMetadata
    {
        public MethodInfo MethodInfo { get; }

        public ResultMetadata Result { get; }

        public IList<ParameterMetadata> Parameters { get; } = new List<ParameterMetadata>();

        public ParameterMetadata TimeoutParameter { get; }

        public ParameterMetadata CancelParameter { get; }

        // shortcut Connection

        // shortcut Tx

        // Method attribute

        public MethodAttribute Method { get; }

        public ProviderAttribute Provider { get; }

        public TimeoutAttribute Timeout { get; }

        // Return value

        //public bool IsAsync { get; set; }
        //public Type ReturnType { get; set; }

        // Parameters

        // TODO Helpers ?

        public MethodMetadata(MethodInfo mi)
        {
            var methodAttribute = mi.GetCustomAttribute<MethodAttribute>();
            if (methodAttribute == null)
            {
                throw new AccessorException($"Method is not supported for generation. type=[{mi.DeclaringType.FullName}], method=[{mi.Name}]");
            }

            MethodInfo = mi;
            Method = methodAttribute;

            Provider = mi.GetCustomAttribute<ProviderAttribute>();
            Timeout = mi.GetCustomAttribute<TimeoutAttribute>();

            Result = new ResultMetadata(mi.ReturnParameter);
            foreach (var pi in mi.GetParameters())
            {
                var parameter = new ParameterMetadata(pi);
                Parameters.Add(parameter);

                if (pi.GetCustomAttribute<TimeoutParameterAttribute>() != null)
                {
                    // TODO type check

                    TimeoutParameter = parameter;
                }

                if (pi.ParameterType == typeof(CancellationToken))
                {
                    CancelParameter = parameter;
                }

                // TODO con

                // TODO tx
            }
        }
    }
}
