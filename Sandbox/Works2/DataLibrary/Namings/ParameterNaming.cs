using System;
using System.Globalization;

namespace DataLibrary.Namings
{
    public sealed class ParameterNaming : IParameterNaming
    {
        private readonly string marker;

        public ParameterNaming(string marker)
        {
            this.marker = marker;
        }

        public string GetName(int index) => String.Concat(marker, "p", index.ToString(CultureInfo.InvariantCulture));
    }
}
