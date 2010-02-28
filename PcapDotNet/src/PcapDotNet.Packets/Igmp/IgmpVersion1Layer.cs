using System;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 1112.
    /// Represents an IGMP version 1 layer.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public abstract class IgmpVersion1Layer : IgmpSimpleLayer
    {
        /// <summary>
        /// The actual time allowed, called the Max Resp Time.
        /// </summary>
        public override TimeSpan MaxResponseTimeValue
        {
            get { return TimeSpan.Zero; }
        }
    }
}