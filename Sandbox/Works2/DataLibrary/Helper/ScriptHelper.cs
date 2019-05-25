namespace DataLibrary.Helper
{
    using System;

    public static class ScriptHelper
    {
        public static bool IsEmpty(string value)
        {
            return (value is null) || (value.Length == 0);
        }
    }
}
