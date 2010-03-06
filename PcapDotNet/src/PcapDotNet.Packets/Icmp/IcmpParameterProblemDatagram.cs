using System;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+---------+------+-----------+
    /// | Bit | 0-7     | 8-15 | 16-31     |
    /// +-----+---------+------+-----------+
    /// | 0   | Type    | Code | Checksum  |
    /// +-----+---------+------+-----------+
    /// | 32  | Pointer | unused           |
    /// +-----+---------+------------------+
    /// | 64  | Internet Header            |
    /// |     | + 64 bits of               |
    /// |     | Original Data Datagram     |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.ParameterProblem)]
    public class IcmpParameterProblemDatagram : IcmpIpV4HeaderPlus64BitsPayloadDatagram
    {
        private static class Offset
        {
            public const int Pointer = 4;
        }

        /// <summary>
        /// The pointer identifies the octet of the original datagram's header where the error was detected (it may be in the middle of an option).  
        /// For example, 1 indicates something is wrong with the Type of Service, and (if there are options present) 20 indicates something is wrong with the type code of the first option.
        /// </summary>
        public byte Pointer
        {
            get { return this[Offset.Pointer]; }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpParameterProblemLayer
                       {
                           Checksum = Checksum,
                           Pointer = Pointer
                       };
        }

        /// <summary>
        /// Valid if the datagram's length is OK, the checksum is correct, the code is in the expected range,
        /// the IPv4 payload contains at least an IPv4 header, the IPv4's payload is in the expected size
        /// and the pointer points to a byte within the IPv4 header.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() && Pointer < IpV4.Length;
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpParameterProblemDatagram(buffer, offset, length);
        }

        private IcmpParameterProblemDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}