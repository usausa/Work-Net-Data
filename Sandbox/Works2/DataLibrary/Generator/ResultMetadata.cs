namespace DataLibrary.Generator
{
    using System.Reflection;

    public class ResultMetadata
    {
        public ParameterInfo Result { get; }

        //public bool IsAsync => Result.ParameterType.

        //public Type ResultType =>

        public ResultMetadata(ParameterInfo pi)
        {
            Result = pi;
        }
    }
}
