namespace DataLibrary.Generator
{
    using System;
    using System.Reflection;

    public sealed class MethodMetadata
    {
        public MethodInfo Method { get; }

        public bool IsAsync { get; set; }

        public Type ReturnType { get; set; }

        public bool IsProviderRequired { get; set; }

        public int? Timeout { get; set; }

        public ParameterInfo TimeoutParameter { get; set; }

        public ParameterInfo CancelParameter { get; set; }

        // TODO Connection

        // TODO Tx

        public MethodMetadata(MethodInfo mi)
        {
            Method = mi;

            // TODO parse
        }
    }
}
