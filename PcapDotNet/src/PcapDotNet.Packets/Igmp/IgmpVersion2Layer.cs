using System;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 2236.
    /// Represents a generic IGMP version 2 datagram.
    /// <seealso cref="IgmpDatagram"/>
    /// </summary>
    public abstract class IgmpVersion2Layer : IgmpVersion1PlusSimpleLayer
    {
        /// <summary>
        /// The actual time allowed, called the Max Resp Time.
        /// </summary>
        public TimeSpan MaxResponseTime { get; set; }

        /// <summary>
        /// The actual time allowed, called the Max Resp Time.
        /// </summary>
        public sealed override TimeSpan MaxResponseTimeValue
        {
            get { return MaxResponseTime; }
        }
    }
}