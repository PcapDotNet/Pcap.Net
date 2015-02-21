using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFCs 792, 4884.
    /// <pre>
    /// +-----+---------+--------+----------+
    /// | Bit | 0-7     | 8-15   | 16-31    |
    /// +-----+---------+--------+----------+
    /// | 0   | Type    | Code   | Checksum |
    /// +-----+---------+--------+----------+
    /// | 32  | Pointer | Length | unused   |
    /// +-----+---------+-------------------+
    /// | 64  | Internet Header             |
    /// |     | + leading octets of         |
    /// |     | original datagram           |
    /// +-----+-----------------------------+
    /// </pre>
    /// </summary>
    [IcmpDatagramRegistration(IcmpMessageType.ParameterProblem)]
    public sealed class IcmpParameterProblemDatagram : IcmpIpV4PayloadDatagram
    {
        private static class Offset
        {
            public const int Pointer = 4;
            public const int OriginalDatagramLength = Pointer + sizeof(byte);
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
        /// Length of the padded "original datagram".
        /// Must divide by 4 and cannot exceed OriginalDatagramLengthMaxValue.
        /// </summary>
        public int OriginalDatagramLength
        {
            get { return this[Offset.OriginalDatagramLength] * sizeof(uint); }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpParameterProblemLayer
                       {
                           Checksum = Checksum,
                           Pointer = Pointer,
                           OriginalDatagramLength = OriginalDatagramLength,
                       };
        }

        /// <summary>
        /// Valid if the datagram's length is OK, the checksum is correct, the code is in the expected range,
        /// the IPv4 payload contains at least an IPv4 header, the IPv4's payload is in the expected size
        /// and the pointer points to a byte within the IPv4 header.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return base.CalculateIsValid() && Pointer < IpV4.Length && OriginalDatagramLength == IpV4.Payload.Length;
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            return new IcmpParameterProblemDatagram(buffer, offset, length);
        }

        internal override void ProcessIpV4Payload(ref IpV4Datagram ipV4)
        {
            if (ipV4.Payload.Length > OriginalDatagramLength)
                ipV4 = new IpV4Datagram(ipV4.Buffer, ipV4.StartOffset, ipV4.HeaderLength + OriginalDatagramLength);
        }

        private IcmpParameterProblemDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}