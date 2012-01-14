using System;
using System.Linq;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Hallam-Baker.
    /// <pre>
    /// +-----+----------+----------+------------+
    /// | bit | 0        | 1-7      | 8-15       |
    /// +-----+----------+----------+------------+
    /// | 0   | Critical | Reserved | Tag Length |
    /// +-----+----------+----------+------------+
    /// | 16  | Tag                              |
    /// | ... |                                  |
    /// +-----+----------------------------------+
    /// | ... | Value                            |
    /// +-----+----------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Caa)]
    public sealed class DnsResourceDataCertificationAuthorityAuthorization : DnsResourceDataSimple, IEquatable<DnsResourceDataCertificationAuthorityAuthorization>
    {
        private static class Offset
        {
            public const int Flags = 0;
            public const int TagLength = Flags + sizeof(byte);
            public const int Tag = TagLength + sizeof(byte);
        }

        private const int ConstantPartLength = Offset.Tag;

        public DnsResourceDataCertificationAuthorityAuthorization(DnsCertificationAuthorityAuthorizationFlags flags, DataSegment tag, DataSegment value)
        {
            if (tag.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException("tag", tag.Length, string.Format("Cannot be longer than {0}", byte.MaxValue));

            Flags = flags;
            Tag = tag;
            Value = value;
        }

        /// <summary>
        /// Flags of the record.
        /// </summary>
        public DnsCertificationAuthorityAuthorizationFlags Flags { get; private set; }

        /// <summary>
        /// The property identifier, a sequence of ASCII characters.
        /// Tag values may contain ASCII characters a through z and the numbers 0 through 9.
        /// Tag values must not contain any other characters.
        /// Matching of tag values is case insensitive.
        /// </summary>
        public DataSegment Tag { get; private set; }

        /// <summary>
        /// Represents the property value.
        /// Property values are encoded as binary values and may employ sub-formats.
        /// </summary>
        public DataSegment Value { get; private set; }

        public bool Equals(DnsResourceDataCertificationAuthorityAuthorization other)
        {
            return other != null &&
                   Flags.Equals(other.Flags) &&
                   Tag.Equals(other.Tag) &&
                   Value.SequenceEqual(other.Value);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataCertificationAuthorityAuthorization);
        }

        internal DnsResourceDataCertificationAuthorityAuthorization()
            : this(0, DataSegment.Empty, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Tag.Length + Value.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Flags, (byte)Flags);
            buffer.Write(offset + Offset.TagLength, (byte)Tag.Length);
            Tag.Write(buffer, offset + Offset.Tag);
            Value.Write(buffer, offset + ConstantPartLength + Tag.Length);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsCertificationAuthorityAuthorizationFlags flags = (DnsCertificationAuthorityAuthorizationFlags)data[Offset.Flags];
            byte tagLength = data[Offset.TagLength];
            int valueOffset = ConstantPartLength + tagLength;
            if (data.Length < valueOffset)
                return null;
            DataSegment tag = data.SubSegment(Offset.Tag, tagLength);
            DataSegment value = data.SubSegment(valueOffset, data.Length - valueOffset);

            return new DnsResourceDataCertificationAuthorityAuthorization(flags, tag, value);
        }
    }
}