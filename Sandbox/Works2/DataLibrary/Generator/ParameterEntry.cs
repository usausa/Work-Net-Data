using System;
using System.Data;

namespace DataLibrary.Generator
{
    internal class ParameterEntry
    {
        public string Source { get; }

        public int Index { get; }

        public Type Type { get; }

        public ParameterDirection Direction { get; }

        public string ParameterName { get; }

        public ParameterType ParameterType { get; }

        public ParameterEntry(string source, int index, Type type, ParameterDirection direction, string parameterName)
        {
            Source = source;
            Index = index;
            Type = type;
            Direction = direction;
            ParameterName = parameterName;

            if (type.IsArray)
            {
                ParameterType = ParameterType.Array;
            }
            else if (GeneratorHelper.IsListParameter(type))
            {
                ParameterType = ParameterType.List;
            }
            else if (GeneratorHelper.IsEnumerableParameter(type))
            {
                ParameterType = ParameterType.Enumerable;
            }
            else
            {
                switch (direction)
                {
                    case ParameterDirection.InputOutput:
                        ParameterType = ParameterType.InputOutput;
                        break;
                    case ParameterDirection.Output:
                        ParameterType = ParameterType.Output;
                        break;
                    case ParameterDirection.ReturnValue:
                        ParameterType = ParameterType.Return;
                        break;
                    default:
                        ParameterType = ParameterType.Input;
                        break;
                }
            }
        }
    }
}
