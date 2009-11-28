using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for TimeSpan.
    /// </summary>
    public static class TimeSpanExtensions
    {
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

        public static TimeSpan Multiply(this TimeSpan timeSpan, double value)
        {
            return TimeSpan.FromTicks((long)(timeSpan.Ticks * value));
        }
    }
}