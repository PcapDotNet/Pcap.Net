using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2402.
    /// <pre>
    /// +-----+-------------+-------------+----------+
    /// | Bit | 0-7         | 8-15        | 16-31    |
    /// +-----+-------------+-------------+----------+
    /// | 0   | Next Header | Payload Len | RESERVED |
    /// +-----+-------------+-------------+----------+
    /// | 32  | Security Parameters Index (SPI)      |
    /// +-----+--------------------------------------+
    /// | 64  | Sequence Number Field                |
    /// +-----+--------------------------------------+
    /// | 96  | Authentication Data (variable)       |
    /// | ... |                                      |
    /// +-----+--------------------------------------+
    /// </pre>
    /// </summary>
    public class IpV6ExtensionHeaderAuthentication : IpV6ExtensionHeader
    {
        private static class Offset
        {
            public const int NextHeader = 0;
            public const int PayloadLength = NextHeader + sizeof(byte);
            public const int SecurityParametersIndex = PayloadLength + sizeof(byte) + sizeof(ushort);
            public const int SequenceNumber = SecurityParametersIndex + sizeof(uint);
            public const int AuthenticationData = SequenceNumber + sizeof(uint);
        }

        public const int MinimumLength = Offset.AuthenticationData;

        public IpV6ExtensionHeaderAuthentication(IpV4Protocol nextHeader, uint securityParametersIndex, uint sequenceNumber, DataSegment authenticationData)
            : base(nextHeader)
        {
            SecurityParametersIndex = securityParametersIndex;
            SequenceNumber = sequenceNumber;
            AuthenticationData = authenticationData;
        }

        /// <summary>
        /// <para>
        /// The SPI is an arbitrary 32-bit value that, in combination with the destination IP address and security protocol (AH),
        /// uniquely identifies the Security Association for this datagram.
        /// The set of SPI values in the range 1 through 255 are reserved by the Internet Assigned Numbers Authority (IANA) for future use;
        /// a reserved SPI value will not normally be assigned by IANA unless the use of the assigned SPI value is specified in an RFC.
        /// It is ordinarily selected by the destination system upon establishment of an SA.
        /// </para>
        /// <para>
        /// The SPI value of zero (0) is reserved for local, implementation-specific use and must not be sent on the wire.
        /// For example, a key management implementation may use the zero SPI value to mean "No Security Association Exists"
        /// during the period when the IPsec implementation has requested that its key management entity establish a new SA,
        /// but the SA has not yet been established.
        /// </para>
        /// </summary>
        public uint SecurityParametersIndex { get; private set; }

        /// <summary>
        /// <para>
        /// Contains a monotonically increasing counter value (sequence number).
        /// It is mandatory and is always present even if the receiver does not elect to enable the anti-replay service for a specific SA.
        /// Processing of the Sequence Number field is at the discretion of the receiver, i.e., the sender must always transmit this field,
        /// but the receiver need not act upon it.
        /// </para>
        /// <para>
        /// The sender's counter and the receiver's counter are initialized to 0 when an SA is established.
        /// (The first packet sent using a given SA will have a Sequence Number of 1.)
        /// If anti-replay is enabled (the default), the transmitted Sequence Number must never be allowed to cycle.
        /// Thus, the sender's counter and the receiver's counter must be reset (by establishing a new SA and thus a new key)
        /// prior to the transmission of the 2^32nd packet on an SA.
        /// </para>
        /// </summary>
        public uint SequenceNumber { get; private set; }

        /// <summary>
        /// This is a variable-length field that contains the Integrity Check Value (ICV) for this packet.
        /// The field must be an integral multiple of 32 bits in length.
        /// This field may include explicit padding.
        /// This padding is included to ensure that the length of the AH header is an integral multiple of 32 bits (IPv4) or 64 bits (IPv6).
        /// All implementations must support such padding.
        /// The authentication algorithm specification must specify the length of the ICV and the comparison rules and processing steps for validation.
        /// </summary>
        public DataSegment AuthenticationData { get; private set; }

        internal static IpV6ExtensionHeaderAuthentication CreateInstance(DataSegment extensionHeaderData, out int numBytesRead)
        {
            if (extensionHeaderData.Length < MinimumLength)
            {
                numBytesRead = 0;
                return null;
            }
            IpV4Protocol nextHeader = (IpV4Protocol)extensionHeaderData[Offset.NextHeader];
            int payloadLength = (extensionHeaderData[Offset.PayloadLength] + 2) * 4;
            if (extensionHeaderData.Length < Offset.AuthenticationData + payloadLength)
            {
                numBytesRead = 0;
                return null;
            }

            uint securityParametersIndex = extensionHeaderData.ReadUInt(Offset.SecurityParametersIndex, Endianity.Big);
            uint sequenceNumber = extensionHeaderData.ReadUInt(Offset.SequenceNumber, Endianity.Big);
            DataSegment authenticationData = extensionHeaderData.Subsegment(Offset.AuthenticationData, payloadLength - Offset.AuthenticationData);
            numBytesRead = payloadLength;

            return new IpV6ExtensionHeaderAuthentication(nextHeader, securityParametersIndex, sequenceNumber, authenticationData);
        }

        public static void GetNextNextHeaderAndLength(DataSegment extensionHeader, out IpV4Protocol? nextNextHeader, out int extensionHeaderLength)
        {
            if (extensionHeader.Length < MinimumLength)
            {
                nextNextHeader = null;
                extensionHeaderLength = extensionHeader.Length;
                return;
            }

            nextNextHeader = (IpV4Protocol)extensionHeader[Offset.NextHeader];
            extensionHeaderLength = (extensionHeader[Offset.PayloadLength] + 2) * 4;
        }

        public override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.AuthenticationHeader; }
        }

        public override int Length
        {
            get { return MinimumLength + AuthenticationData.Length; }
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.NextHeader, (byte)NextHeader);
            int length = Length;
            buffer.Write(offset + Offset.PayloadLength, (byte)((length / 4) - 2));
            buffer.Write(offset + Offset.SecurityParametersIndex, SecurityParametersIndex, Endianity.Big);
            buffer.Write(offset + Offset.SequenceNumber, SequenceNumber, Endianity.Big);
            AuthenticationData.Write(buffer, offset + Offset.AuthenticationData);
            offset += length;
        }
    }
}