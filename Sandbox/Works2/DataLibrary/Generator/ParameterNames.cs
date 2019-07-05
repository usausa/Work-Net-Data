namespace DataLibrary.Generator
{
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class ParameterNames
    {
        private static readonly string[] names;

        static ParameterNames()
        {
            names = Enumerable.Range(0, 256).Select(x => $"p{x}").ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetParameterName(int index)
        {
            return index < names.Length ? names[index] : $"p{index}";
        }
    }
}
