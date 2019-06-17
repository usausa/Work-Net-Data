namespace DataLibrary.Engine
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ConvertHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Convert<T>(object source, Func<object, object> converter)
        {
            if (source is T value)
            {
                return value;
            }

            if (source is DBNull)
            {
                return default;
            }

            return (T)converter(source);
        }
    }
}
