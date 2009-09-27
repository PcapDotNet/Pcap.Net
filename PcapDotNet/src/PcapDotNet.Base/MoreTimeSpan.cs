using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PcapDotNet.Base
{
    public static class MoreTimeSpan
    {
        public static TimeSpan Divide(this TimeSpan timeSpan, double value)
        {
            return TimeSpan.FromTicks((long)(timeSpan.Ticks / value));
        }
    }
}