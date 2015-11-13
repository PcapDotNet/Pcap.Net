using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// Base class for all IGMP layers of version 1 or higher.
    /// </summary>
    public abstract class IgmpVersion1PlusLayer : IgmpLayer
    {
        /// <summary>
        /// The actual time allowed, called the Max Resp Time.
        /// </summary>
        public abstract TimeSpan MaxResponseTimeValue { get; }

        /// <summary>
        /// true iff the fields that are not mutual to all IGMP layers are equal.
        /// </summary>
        protected override sealed bool EqualsVersionSpecific(IgmpLayer other)
        {
            return EqualsVersionSpecific(other as IgmpVersion1PlusLayer);
        }

        /// <summary>
        /// true iff the fields that are not mutual to all IGMP version 1+ layers are equal.
        /// </summary>
        protected abstract bool EqualFields(IgmpVersion1PlusLayer other);

        private bool EqualsVersionSpecific(IgmpVersion1PlusLayer other)
        {
            return other != null &&
                   EqualMaxResponseTime(MaxResponseTimeValue, other.MaxResponseTimeValue) &&
                   EqualFields(other);
        }

        private static bool EqualMaxResponseTime(TimeSpan value1, TimeSpan value2)
        {
            return value1.Divide(2) <= value2 && value1.Multiply(2) >= value2;
        }
    }
}