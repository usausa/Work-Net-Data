namespace DataLibrary.Generator
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    public class ResultMetadata
    {
        public ParameterInfo Result { get; }

        public bool IsAsync => Result.ParameterType.GetMethod(nameof(Task.GetAwaiter)) != null;

        public Type ResultType
        {
            get
            {
                if (IsAsync)
                {
                    if (Result.ParameterType.IsGenericType)
                    {
                        return Result.ParameterType.GetGenericArguments()[0];
                    }

                    return typeof(void);
                }

                return Result.ParameterType;
            }
        }

        public ResultMetadata(ParameterInfo pi)
        {
            Result = pi;
        }
    }
}
