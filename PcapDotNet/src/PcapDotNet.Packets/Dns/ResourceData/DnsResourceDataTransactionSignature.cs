using System;
using System.Globalization;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2845.
    /// <pre>
    /// +------+-------------+----------+-----------+
    /// | bit  | 0-15        | 16-31    | 32-47     |
    /// +------+-------------+----------+-----------+
    /// | 0    | Algorithm Name                     |
    /// | ...  |                                    |
    /// +------+------------------------------------+
    /// | X    | Time Signed                        |
    /// +------+-------------+----------+-----------+
    /// | X+48 | Fudge       | MAC Size | MAC       |
    /// +------+-------------+----------+           |
    /// | ...  |                                    |
    /// +------+-------------+----------+-----------+
    /// | Y    | Original ID | Error    | Other Len |
    /// +------+-------------+----------+-----------+
    /// | Y+48 | Other Data                         |
    /// | ...  |                                    |
    /// +------+------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.TransactionSignature)]
    public sealed class DnsResourceDataTransactionSignature : DnsResourceData, IEquatable<DnsResourceDataTransactionSignature>
    {
        private static class OffsetAfterAlgorithm
        {
            public const int TimeSigned = 0;
            public const int Fudge = TimeSigned + UInt48.SizeOf;
            public const int MessageAuthenticationCodeSize = Fudge + sizeof(ushort);
            public const int MessageAuthenticationCode = MessageAuthenticationCodeSize + sizeof(ushort);
        }

        private static class OffsetAfterMessageAuthenticationCode
        {
            public const int OriginalId = 0;
            public const int Error = OriginalId + sizeof(ushort);
            public const int OtherLength = Error + sizeof(ushort);
            public const int OtherData = OtherLength + sizeof(ushort);
        }

        private const int ConstantPartLength = OffsetAfterAlgorithm.MessageAuthenticationCode + OffsetAfterMessageAuthenticationCode.OtherData;

        /// <summary>
        /// Constructs an instance out of the algorithm time signed, fudge, message authentication code, original ID, error and other fields.
        /// </summary>
        /// <param name="algorithm">Name of the algorithm in domain name syntax.</param>
        /// <param name="timeSigned">Seconds since 1-Jan-70 UTC.</param>
        /// <param name="fudge">Seconds of error permitted in Time Signed.</param>
        /// <param name="messageAuthenticationCode">Defined by Algorithm Name.</param>
        /// <param name="originalId">Original message ID.</param>
        /// <param name="error">RCODE covering TSIG processing.</param>
        /// <param name="other">Empty unless Error == BADTIME.</param>
        public DnsResourceDataTransactionSignature(DnsDomainName algorithm, UInt48 timeSigned, ushort fudge, DataSegment messageAuthenticationCode,
                                                   ushort originalId, DnsResponseCode error, DataSegment other)
        {
            if (messageAuthenticationCode == null)
                throw new ArgumentNullException("messageAuthenticationCode");
            if (other == null)
                throw new ArgumentNullException("other");

            if (messageAuthenticationCode.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("messageAuthenticationCode", messageAuthenticationCode.Length,
                                                      string.Format(CultureInfo.InvariantCulture, "Cannot be longer than {0}", ushort.MaxValue));
            if (other.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("other", other.Length,
                                                      string.Format(CultureInfo.InvariantCulture, "Cannot be longer than {0}", ushort.MaxValue));

            Algorithm = algorithm;
            TimeSigned = timeSigned;
            Fudge = fudge;
            MessageAuthenticationCode = messageAuthenticationCode;
            OriginalId = originalId;
            Error = error;
            Other = other;
        }

        /// <summary>
        /// Name of the algorithm in domain name syntax.
        /// </summary>
        public DnsDomainName Algorithm { get; private set; }

        /// <summary>
        /// Seconds since 1-Jan-70 UTC.
        /// </summary>
        public UInt48 TimeSigned { get; private set; }

        /// <summary>
        /// Seconds of error permitted in Time Signed.
        /// </summary>
        public ushort Fudge { get; private set; }

        /// <summary>
        /// Defined by Algorithm Name.
        /// </summary>
        public DataSegment MessageAuthenticationCode { get; private set; }

        /// <summary>
        /// Original message ID.
        /// </summary>
        public ushort OriginalId { get; private set; }

        /// <summary>
        /// RCODE covering TSIG processing.
        /// </summary>
        public DnsResponseCode Error { get; private set; }

        /// <summary>
        /// Empty unless Error == BADTIME.
        /// </summary>
        public DataSegment Other { get; private set; }

        /// <summary>
        /// Two DnsResourceDataTransactionSignature are equal iff their algorithm time signed, fudge, message authentication code, original ID, error
        /// and other fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataTransactionSignature other)
        {
            return other != null &&
                   Algorithm.Equals(other.Algorithm) &&
                   TimeSigned.Equals(other.TimeSigned) &&
                   Fudge.Equals(other.Fudge) &&
                   MessageAuthenticationCode.Equals(other.MessageAuthenticationCode) &&
                   OriginalId.Equals(other.OriginalId) &&
                   Error.Equals(other.Error) &&
                   Other.Equals(other.Other);
        }

        /// <summary>
        /// Two DnsResourceDataTransactionSignature are equal iff their algorithm time signed, fudge, message authentication code, original ID, error
        /// and other fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataTransactionSignature);
        }

        /// <summary>
        /// A hash code of the combination of the algorithm time signed, fudge, message authentication code, original ID, error and other fields.
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(Algorithm, TimeSigned, BitSequence.Merge(Fudge, OriginalId), MessageAuthenticationCode, Error, Other);
        }

        internal DnsResourceDataTransactionSignature()
            : this(DnsDomainName.Root, 0, 0, DataSegment.Empty, 0, DnsResponseCode.NoError, DataSegment.Empty)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return Algorithm.GetLength(compressionData, offsetInDns) + ConstantPartLength + MessageAuthenticationCode.Length + Other.Length;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int algorithmLength = Algorithm.Write(buffer, dnsOffset, compressionData, offsetInDns);
            int offset = dnsOffset + offsetInDns + algorithmLength;
            buffer.Write(offset + OffsetAfterAlgorithm.TimeSigned, TimeSigned, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.Fudge, Fudge, Endianity.Big);
            buffer.Write(offset + OffsetAfterAlgorithm.MessageAuthenticationCodeSize, (ushort)MessageAuthenticationCode.Length, Endianity.Big);
            MessageAuthenticationCode.Write(buffer, offset + OffsetAfterAlgorithm.MessageAuthenticationCode);

            offset += OffsetAfterAlgorithm.MessageAuthenticationCode + MessageAuthenticationCode.Length;
            buffer.Write(offset + OffsetAfterMessageAuthenticationCode.OriginalId, OriginalId, Endianity.Big);
            buffer.Write(offset + OffsetAfterMessageAuthenticationCode.Error, (ushort)Error, Endianity.Big);
            buffer.Write(offset + OffsetAfterMessageAuthenticationCode.OtherLength, (ushort)Other.Length, Endianity.Big);
            Other.Write(buffer, offset + OffsetAfterMessageAuthenticationCode.OtherData);

            return algorithmLength + ConstantPartLength + MessageAuthenticationCode.Length + Other.Length;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength + DnsDomainName.RootLength)
                return null;

            DnsDomainName algorithm;
            int algorithmLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length - ConstantPartLength, out algorithm, out algorithmLength))
                return null;
            offsetInDns += algorithmLength;
            length -= algorithmLength;

            if (length < ConstantPartLength)
                return null;

            UInt48 timeSigned = dns.ReadUInt48(offsetInDns + OffsetAfterAlgorithm.TimeSigned, Endianity.Big);
            ushort fudge  = dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.Fudge, Endianity.Big);
            int messageAuthenticationCodeLength = dns.ReadUShort(offsetInDns + OffsetAfterAlgorithm.MessageAuthenticationCodeSize, Endianity.Big);
            if (length < ConstantPartLength + messageAuthenticationCodeLength)
                return null;
            DataSegment messageAuthenticationCode = dns.Subsegment(offsetInDns + OffsetAfterAlgorithm.MessageAuthenticationCode, messageAuthenticationCodeLength);
            int totalReadAfterAlgorithm = OffsetAfterAlgorithm.MessageAuthenticationCode + messageAuthenticationCodeLength;
            offsetInDns += totalReadAfterAlgorithm;
            length -= totalReadAfterAlgorithm;

            ushort originalId = dns.ReadUShort(offsetInDns + OffsetAfterMessageAuthenticationCode.OriginalId, Endianity.Big);
            DnsResponseCode error = (DnsResponseCode)dns.ReadUShort(offsetInDns + OffsetAfterMessageAuthenticationCode.Error, Endianity.Big);
            int otherLength = dns.ReadUShort(offsetInDns + OffsetAfterMessageAuthenticationCode.OtherLength, Endianity.Big);
            if (length != OffsetAfterMessageAuthenticationCode.OtherData + otherLength)
                return null;
            DataSegment other = dns.Subsegment(offsetInDns + OffsetAfterMessageAuthenticationCode.OtherData, otherLength);

            return new DnsResourceDataTransactionSignature(algorithm, timeSigned, fudge, messageAuthenticationCode, originalId, error, other);
        }
    }
}