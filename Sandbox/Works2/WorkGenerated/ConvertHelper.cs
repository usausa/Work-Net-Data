using System;
using System.Runtime.CompilerServices;

namespace WorkGenerated
{
    public static class ConvertHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Convert<T>(object value)
        {
            if (value is T value1)
            {
                return value1;
            }

            if (value is DBNull)
            {
                return default;
            }

            return (T)System.Convert.ChangeType(value, typeof(T));
        }
    }
}
