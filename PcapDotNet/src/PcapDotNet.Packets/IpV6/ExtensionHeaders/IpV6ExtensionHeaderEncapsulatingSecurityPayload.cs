using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2406.
    /// <pre>
    /// +-----+------------+-------------+
    /// | Bit | 0-7        | 8-15        |
    /// +-----+------------+-------------+
    /// | 0   | Security Parameters      |
    /// |     | Index (SPI)              |
    /// +-----+--------------------------+
    /// | 32  | Sequence Number          |
    /// |     |                          |
    /// +-----+--------------------------+
    /// | 64  | Payload Data             |
    /// | ... |                          |
    /// +-----+--------------------------+
    /// |     | Padding                  |
    /// | ... |                          |
    /// +-----+------------+-------------+
    /// |     | Pad Length | Next Header |
    /// +-----+------------+-------------+
    /// |     | Authentication Data      |
    /// | ... |                          |
    /// +-----+--------------------------+
    /// </pre>
    /// 
    /// <pre>
    /// +-----+------------+-------------+
    /// | Bit | 0-7        | 8-15        |
    /// +-----+------------+-------------+
    /// | 0   | Security Parameters      |
    /// |     | Index (SPI)              |
    /// +-----+--------------------------+
    /// | 32  | Sequence Number          |
    /// |     |                          |
    /// +-----+--------------------------+
    /// | 64  | Encrypted Data           |
    /// | ... |                          |
    /// +-----+--------------------------+
    /// |     | Authentication Data      |
    /// | ... |                          |
    /// +-----+--------------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6ExtensionHeaderEncapsulatingSecurityPayload : IpV6ExtensionHeader, IEquatable<IpV6ExtensionHeaderEncapsulatingSecurityPayload>
    {
        private static class Offset
        {
            public const int SecurityParametersIndex = 0;
            public const int SequenceNumber = SecurityParametersIndex + sizeof(uint);
            public const int PayloadData = SequenceNumber + sizeof(uint);
        }

        public const int MinimumLength = Offset.PayloadData;

        public IpV6ExtensionHeaderEncapsulatingSecurityPayload(uint securityParametersIndex, uint sequenceNumber, DataSegment encryptedDataAndAuthenticationData)
            : base(null)
        {
            SecurityParametersIndex = securityParametersIndex;
            SequenceNumber = sequenceNumber;
            EncryptedDataAndAuthenticationData = encryptedDataAndAuthenticationData;
        }

        public override IpV4Protocol Protocol
        {
            get { return IpV4Protocol.EncapsulatingSecurityPayload; }
        }

        public override int Length
        {
            get { return MinimumLength + EncryptedDataAndAuthenticationData.Length; }
        }

        public override bool IsValid
        {
            get { return true; }
        }

        public override bool Equals(IpV6ExtensionHeader other)
        {
            return Equals(other as IpV6ExtensionHeaderEncapsulatingSecurityPayload);
        }

        public bool Equals(IpV6ExtensionHeaderEncapsulatingSecurityPayload other)
        {
            return other != null &&
                   SecurityParametersIndex == other.SecurityParametersIndex && SequenceNumber == other.SequenceNumber &&
                   EncryptedDataAndAuthenticationData.Equals(other.EncryptedDataAndAuthenticationData);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(SecurityParametersIndex, SequenceNumber, EncryptedDataAndAuthenticationData);
        }

        /// <summary>
        /// <para>
        /// The SPI is an arbitrary 32-bit value that, in combination with the destination IP address and security protocol (ESP),
        /// uniquely identifies the Security Association for this datagram.
        /// The set of SPI values in the range 1 through 255 are reserved by the Internet Assigned Numbers Authority (IANA) for future use;
        /// a reserved SPI value will not normally be assigned by IANA unless the use of the assigned SPI value is specified in an RFC.
        /// It is ordinarily selected by the destination system upon establishment of an SA (see the Security Architecture document for more details).
        /// The SPI field is mandatory.
        /// </para>
        /// <para>
        /// The SPI value of zero (0) is reserved for local, implementation-specific use and must not be sent on the wire.
        /// For example, a key management implementation MAY use the zero SPI value to mean "No Security Association Exists" 
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
        /// Contains the encrupted Payload Data, Padding, Pad Length and Next Header and the Authentication Data.
        /// <para>
        /// Payload Data is a variable-length field containing data described by the Next Header field.
        /// The Payload Data field is mandatory and is an integral number of bytes in length.
        /// If the algorithm used to encrypt the payload requires cryptographic synchronization data, e.g., an Initialization Vector (IV),
        /// then this data may be carried explicitly in the Payload field.
        /// Any encryption algorithm that requires such explicit, per-packet synchronization data must indicate the length, any structure for such data,
        /// and the location of this data as part of an RFC specifying how the algorithm is used with ESP.
        /// If such synchronization data is implicit, the algorithm for deriving the data must be part of the RFC.
        /// </para>
        /// <para>
        /// The sender may add 0-255 bytes of padding.
        /// Inclusion of the Padding field in an ESP packet is optional, but all implementations must support generation and consumption of padding.
        /// </para>
        /// <para>
        /// The Pad Length field indicates the number of pad bytes immediately preceding it.
        /// The range of valid values is 0-255, where a value of zero indicates that no Padding bytes are present.
        /// The Pad Length field is mandatory.
        /// </para>
        /// <para>
        /// The Next Header is an 8-bit field that identifies the type of data contained in the Payload Data field, 
        /// e.g., an extension header in IPv6 or an upper layer protocol identifier.
        /// The Next Header field is mandatory.
        /// </para>
        /// <para>
        /// The Authentication Data is a variable-length field containing an Integrity Check Value (ICV) 
        /// computed over the ESP packet minus the Authentication Data.
        /// The length of the field is specified by the authentication function selected.
        /// The Authentication Data field is optional, and is included only if the authentication service has been selected for the SA in question.
        /// The authentication algorithm specification must specify the length of the ICV and the comparison rules and processing steps for validation.
        /// </para>
        /// </summary>
        public DataSegment EncryptedDataAndAuthenticationData { get; private set; }

        internal static void GetNextNextHeaderAndLength(DataSegment extensionHeader, out IpV4Protocol? nextNextHeader, out int extensionHeaderLength)
        {
            nextNextHeader = null;
            extensionHeaderLength = extensionHeader.Length;
        }

        internal static IpV6ExtensionHeaderEncapsulatingSecurityPayload CreateInstance(DataSegment extensionHeaderData, out int numBytesRead)
        {
            if (extensionHeaderData.Length < MinimumLength)
            {
                numBytesRead = 0;
                return null;
            }
            uint securityParametersIndex = extensionHeaderData.ReadUInt(Offset.SecurityParametersIndex, Endianity.Big);
            uint sequenceNumber = extensionHeaderData.ReadUInt(Offset.SequenceNumber, Endianity.Big);
            DataSegment encryptedDataAndAuthenticationData = extensionHeaderData.Subsegment(Offset.PayloadData, extensionHeaderData.Length - Offset.PayloadData);
            numBytesRead = extensionHeaderData.Length;

            return new IpV6ExtensionHeaderEncapsulatingSecurityPayload(securityParametersIndex, sequenceNumber, encryptedDataAndAuthenticationData);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.SecurityParametersIndex, SecurityParametersIndex, Endianity.Big);
            buffer.Write(offset + Offset.SequenceNumber, SequenceNumber, Endianity.Big);
            EncryptedDataAndAuthenticationData.Write(buffer, offset + Offset.PayloadData);
            offset += Length;
        }
    }
}