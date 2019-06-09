﻿namespace DataLibrary.Engine
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    [Obsolete]
    public static class ConvertHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Convert<T>(object value)
        {
            if (value is T scalar)
            {
                return scalar;
            }

            if (value is DBNull)
            {
                return default;
            }

            return (T)System.Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}