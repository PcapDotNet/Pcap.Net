using System;
using PcapDotNet.Base;
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
    public sealed class IpV6ExtensionHeaderAuthentication : IpV6ExtensionHeader, IEquatable<IpV6ExtensionHeaderAuthentication>
    {
        private static class Offset
        {
            public const int NextHeader = 0;
            public const int PayloadLength = NextHeader + sizeof(byte);
            public const int SecurityParametersIndex = PayloadLength + sizeof(byte) + sizeof(ushort);
            public const int SequenceNumber = SecurityParametersIndex + sizeof(uint);
            public const int AuthenticationData = SequenceNumber + sizeof(uint);
        }

        /// <summary>
        /// The minimum number of bytes the extension header takes.
        /// </summary>
        public const int MinimumLength = Offset.AuthenticationData;

        /// <summary>
        /// Creates an instance from next header, security parameter index, sequence number and authenticator.
        /// </summary>
        /// <param name="nextHeader">
        /// Identifies the type of header immediately following this extension header.
        /// </param>
        /// <param name="securityParametersIndex">
        /// The SPI is an arbitrary 32-bit value that, in combination with the destination IP address and security protocol (AH),
        /// uniquely identifies the Security Association for this datagram.
        /// The set of SPI values in the range 1 through 255 are reserved by the Internet Assigned Numbers Authority (IANA) for future use;
        /// a reserved SPI value will not normally be assigned by IANA unless the use of the assigned SPI value is specified in an RFC.
        /// It is ordinarily selected by the destination system upon establishment of an SA.
        /// </param>
        /// <param name="sequenceNumber">
        /// Contains a monotonically increasing counter value (sequence number).
        /// It is mandatory and is always present even if the receiver does not elect to enable the anti-replay service for a specific SA.
        /// Processing of the Sequence Number field is at the discretion of the receiver, i.e., the sender must always transmit this field,
        /// but the receiver need not act upon it.
        /// </param>
        /// <param name="authenticationData">
        /// This is a variable-length field that contains the Integrity Check Value (ICV) for this packet.
        /// The field must be an integral multiple of 32 bits in length.
        /// This field may include explicit padding.
        /// This padding is included to ensure that the length of the AH header is an integral multiple of 32 bits (IPv4) or 64 bits (IPv6).
        /// All implementations must support such padding.
        /// The authentication algorithm specification must specify the length of the ICV and the comparison rules and processing steps for validation.
        /// </param>
        public IpV6ExtensionHeaderAuthentication(IpV4Protocol nextHeader, uint securityParametersIndex, uint sequenceNumber, DataSegment authenticationData)
            : base(nextHeader)
        {
            if (authenticationData.Length % 4 != 0)
            {
                throw new ArgumentException(
                    string.Format("Authentication Data must be an integral multiple of 4 byte in length, and not {0}.", authenticationData.Length),
                    "authenticationData");
            }
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

        /// <summary>
        /// Identifies the type of this extension header.
        /// </summary>
        public override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.AuthenticationHeader; }
        }

        /// <summary>
        /// The number of bytes this extension header takes.
        /// </summary>
        public override int Length
        {
            get { return MinimumLength + AuthenticationData.Length; }
        }

        /// <summary>
        /// True iff the extension header parsing didn't encounter an issue.
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }

        /// <summary>
        /// True iff the given extension header is equal to this extension header.
        /// </summary>
        public override bool Equals(IpV6ExtensionHeader other)
        {
            return Equals(other as IpV6ExtensionHeaderAuthentication);
        }

        /// <summary>
        /// True iff the given extension header is equal to this extension header.
        /// </summary>
        public bool Equals(IpV6ExtensionHeaderAuthentication other)
        {
            return other != null &&
                   NextHeader == other.NextHeader && SecurityParametersIndex == other.SecurityParametersIndex && SequenceNumber == other.SequenceNumber &&
                   AuthenticationData.Equals(other.AuthenticationData);
        }

        /// <summary>
        /// Returns a hash code of the extension header.
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(NextHeader, SecurityParametersIndex, SequenceNumber, AuthenticationData);
        }

        internal static IpV6ExtensionHeaderAuthentication CreateInstance(DataSegment extensionHeaderData, out int numBytesRead)
        {
            if (extensionHeaderData.Length < MinimumLength)
            {
                numBytesRead = 0;
                return null;
            }
            IpV4Protocol nextHeader = (IpV4Protocol)extensionHeaderData[Offset.NextHeader];
            int payloadLength = (extensionHeaderData[Offset.PayloadLength] + 2) * 4;
            if (extensionHeaderData.Length < payloadLength || payloadLength < MinimumLength)
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

        internal static void GetNextNextHeaderAndLength(DataSegment extensionHeader, out IpV4Protocol? nextNextHeader, out int extensionHeaderLength)
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