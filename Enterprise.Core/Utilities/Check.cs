using System;

namespace Enterprise.Core.Utilities
{
    internal static class Check
    {
        public static T NotNull<T>(
            T value)
            where T :class
        {
            return NotNull(value, nameof(value));
        }

        public static T NotNull<T>(
            T value,
            string parameterName)
            where T : class
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static T? NotNull<T>(
            T? value)
            where T : struct
        {
            return NotNull(value, nameof(value));
        }

        public static T? NotNull<T>(
            T? value,
            string parameterName)
            where T : struct
        {
            if (!value.HasValue)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}
