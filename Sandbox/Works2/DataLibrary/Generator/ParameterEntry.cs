using System;
using System.Data;

namespace DataLibrary.Generator
{
    public class ParameterEntry
    {
        public string Source { get; }

        public Type Type { get; }

        public ParameterDirection Direction { get; }

        public int ParameterIndex { get; }

        public ParameterEntry(string source, Type type, ParameterDirection direction, int parameterIndex)
        {
            Source = source;
            Type = type;
            Direction = direction;
            ParameterIndex = parameterIndex;
        }
    }
}
