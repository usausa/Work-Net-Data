using System;
using System.Collections.Generic;
using System.Text;

namespace ReaderBenchmark
{
    public struct ColumnInfo
    {
        public string Name { get; }

        public Type Type { get; }

        public ColumnInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
