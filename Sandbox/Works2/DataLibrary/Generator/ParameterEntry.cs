using System;
using System.Data;

namespace DataLibrary.Generator
{
    public class ParameterEntry
    {
        public string Source { get; }

        public int Index { get; }

        public Type Type { get; }

        public ParameterDirection Direction { get; }

        public string ParameterName { get; }

        public ParameterEntry(string source, int index, Type type, ParameterDirection direction, string parameterName)
        {
            Source = source;
            Index = index;
            Type = type;
            Direction = direction;
            ParameterName = parameterName;
        }
    }
}
