using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for TimeSpan.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Represents the number of ticks in 1 microsecond. This field is constant.
        /// </summary>
        public const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

        /// <summary>
        /// Divides the TimeSpan by a given value.
        /// </summary>
        /// <param name="timeSpan">The TimeSpan to divide.</param>
        /// <param name="value">The value to divide the TimeSpan by.</param>
        /// <returns>A TimeSpan value equals to the given TimeSpan divided by the given value.</returns>
        public static TimeSpan Divide(this TimeSpan timeSpan, double value)
        {
            return TimeSpan.FromTicks((long)(timeSpan.Ticks / value));
        }

        /// <summary>
        /// Multiplies the TimeSpan by a given value.
        /// </summary>
        /// <param name="timeSpan">The TimeSpan to multiply.</param>
        /// <param name="value">The value to multiply the TimeSpan by.</param>
        /// <returns>A TimeSpan value equals to the given TimeSpan multiplied by the given value.</returns>
        public static TimeSpan Multiply(this TimeSpan timeSpan, double value)
        {
            return TimeSpan.FromTicks((long)(timeSpan.Ticks * value));
        }
    }
}