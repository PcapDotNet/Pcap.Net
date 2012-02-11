using System;
using System.Globalization;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1706.
    /// <pre>
    /// +-----+-----+----------------------+----------+-----------+
    /// | bit | 0-7 | 8-7+X                | 8+X-55+X | 56+X-63+X |
    /// +-----+-----+----------------------+----------+-----------+
    /// | 0   | AFI | Domain Specific Area | ID       | Sel       |
    /// +-----+-----+-----+----------------+----------+-----------+
    /// | 0   | AFI | IDI | HO-DSP         | ID       | Sel       |
    /// +-----+-----+-----+----------------+----------+-----------+
    /// | 0   | Area Address               | ID       | Sel       |
    /// +-----+-----------+----------------+----------+-----------+
    /// | 0   | IDP       | DSP                                   |
    /// +-----+-----------+---------------------------------------+
    /// </pre>
    /// IDP is Initial Domain Part.
    /// DSP is Domain Specific Part.
    /// HO-DSP may use any format as defined by the authority identified by IDP.
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NetworkServiceAccessPoint)]
    public sealed class DnsResourceDataNetworkServiceAccessPoint : DnsResourceDataSimple, IEquatable<DnsResourceDataNetworkServiceAccessPoint>
    {
        private static class Offset
        {
            public const int AreaAddress = 0;
        }

        private static class OffsetAfterArea
        {
            public const int SystemIdentifier = 0;
            public const int Selector = SystemIdentifier + UInt48.SizeOf;
        }

        private const int MinAreaAddressLength = sizeof(byte);

        private const int ConstantPartLength = MinAreaAddressLength + OffsetAfterArea.Selector + sizeof(byte);

        public DnsResourceDataNetworkServiceAccessPoint(DataSegment areaAddress, UInt48 systemIdentifier, byte selector)
        {
            if (areaAddress == null)
                throw new ArgumentNullException("areaAddress");

            if (areaAddress.Length < MinAreaAddressLength)
                throw new ArgumentOutOfRangeException("areaAddress", areaAddress.Length,
                                                      string.Format(CultureInfo.InvariantCulture, "Area Address length must be at least {0}.",
                                                                    MinAreaAddressLength));
            AreaAddress = areaAddress;
            SystemIdentifier = systemIdentifier;
            Selector = selector;
        }

        /// <summary>
        /// Authority and Format Identifier.
        /// </summary>
        public byte AuthorityAndFormatIdentifier { get { return AreaAddress[0]; } }

        /// <summary>
        /// The combination of [IDP, HO-DSP] identify both the routing domain and the area within the routing domain.
        /// Hence the combination [IDP, HO-DSP] is called the "Area Address".
        /// All nodes within the area must have same Area address.
        /// </summary>
        public DataSegment AreaAddress { get; private set; }

        /// <summary>
        /// System Identifier.
        /// </summary>
        public UInt48 SystemIdentifier { get; private set; }

        /// <summary>
        /// NSAP Selector
        /// </summary>
        public byte Selector { get; private set; }

        public bool Equals(DnsResourceDataNetworkServiceAccessPoint other)
        {
            return other != null &&
                   AreaAddress.Equals(other.AreaAddress) &&
                   SystemIdentifier.Equals(other.SystemIdentifier) &&
                   Selector.Equals(other.Selector);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataNetworkServiceAccessPoint);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(AreaAddress, SystemIdentifier, Selector);
        }

        internal DnsResourceDataNetworkServiceAccessPoint()
            : this(new DataSegment(new byte[MinAreaAddressLength]), 0, 0)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + AreaAddress.Length - MinAreaAddressLength;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            AreaAddress.Write(buffer, offset + Offset.AreaAddress);

            int afterAreaOffset = offset + AreaAddress.Length;
            buffer.Write(afterAreaOffset + OffsetAfterArea.SystemIdentifier, SystemIdentifier, Endianity.Big);
            buffer.Write(afterAreaOffset + OffsetAfterArea.Selector, Selector);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DataSegment areaAddress = data.Subsegment(Offset.AreaAddress, MinAreaAddressLength + data.Length - ConstantPartLength);

            int afterAreaOffset = areaAddress.Length;
            UInt48 systemIdentifier = data.ReadUInt48(afterAreaOffset + OffsetAfterArea.SystemIdentifier, Endianity.Big);
            byte selector = data[afterAreaOffset + OffsetAfterArea.Selector];

            return new DnsResourceDataNetworkServiceAccessPoint(areaAddress, systemIdentifier, selector);
        }
    }
}