using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a new DateTime that adds the specified number of microseconds to the value of this instance.
        /// </summary>
        /// <param name="dateTime">The DateTime to add microseconds to.</param>
        /// <param name="value">A number of whole and fractional microseconds. The value parameter can be negative or positive. Note that this value is rounded to the nearest integer.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of microseconds represented by value.</returns>
        public static DateTime AddMicroseconds(this DateTime dateTime, double value)
        {
            const long MaxValue = long.MaxValue / TimeSpanExtensions.TicksPerMicrosecond;
            if (value > MaxValue)
            {
                throw new ArgumentOutOfRangeException("value", value,
                                                      string.Format("Value cannot be bigger than {0}", MaxValue));
            }

            const long MinValue = long.MinValue / TimeSpanExtensions.TicksPerMicrosecond;
            if (value < MinValue)
            {
                throw new ArgumentOutOfRangeException("value", value,
                                                      string.Format("Value cannot be smaller than {0}", MinValue));
            }

            long roundedValue = (long)Math.Round(value);
            long ticks = roundedValue * TimeSpanExtensions.TicksPerMicrosecond;
            return dateTime.AddTicks(ticks);
        }
    }
}