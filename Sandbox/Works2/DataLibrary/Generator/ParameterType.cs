namespace DataLibrary.Generator
{
    internal enum ParameterType
    {
        Array,
        List,
        Enumerable,
        Input,
        InputOutput,
        Output,
        Return
    }

    internal static class ParameterTypeExtensions
    {
        public static bool IsOutType(this ParameterType parameterType)
        {
            return parameterType == ParameterType.InputOutput ||
                   parameterType == ParameterType.Output ||
                   parameterType == ParameterType.Return;
        }
    }

}
