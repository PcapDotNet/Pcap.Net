using System;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// <pre>
    /// +-----+-------+------------+
    /// | bit | 0-15  | 16-31      |
    /// +-----+-------+------------+
    /// | 0   | Order | Preference |
    /// +-----+-------+------------+
    /// | 32  | FLAGS              |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | SERVICES           |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | REGEXP             |
    /// | ... |                    |
    /// +-----+--------------------+
    /// |     | REPLACEMENT        |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.NaPtr)]
    public sealed class DnsResourceDataNamingAuthorityPointer : DnsResourceDataNoCompression, IEquatable<DnsResourceDataNamingAuthorityPointer>
    {
        private static class Offset
        {
            public const int Order = 0;
            public const int Preference = Order + sizeof(ushort);
            public const int Flags = Preference + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.Flags;

        public DnsResourceDataNamingAuthorityPointer(ushort order, ushort preference, DataSegment flags, DataSegment services, DataSegment regexp, DnsDomainName replacement)
        {
            if (!IsLegalFlags(flags))
            {
                throw new ArgumentException(
                    string.Format("Flags ({0}) contain a non [a-zA-Z0-9] character.",
                                  Encoding.ASCII.GetString(flags.Buffer, flags.StartOffset, flags.Length)),
                    "flags");
            }

            Order = order;
            Preference = preference;
            Flags = flags.All(flag => flag < 'a' || flag > 'z') && flags.IsStrictOrdered()
                        ? flags
                        : new DataSegment(flags.Select(flag => flag >= 'a' && flag <= 'z' ? (byte)(flag + 'A' - 'a') : flag)
                                              .Distinct().OrderBy(flag => flag).ToArray());
            Services = services;
            Regexp = regexp;
            Replacement = replacement;
        }

        /// <summary>
        /// A 16-bit unsigned integer specifying the order in which the NAPTR records MUST be processed in order to accurately represent the ordered list of Rules.
        /// The ordering is from lowest to highest.
        /// If two records have the same order value then they are considered to be the same rule and should be selected based on the combination of the Preference values and Services offered.
        /// </summary>
        public ushort Order { get; private set; }

        /// <summary>
        /// Although it is called "preference" in deference to DNS terminology, this field is equivalent to the Priority value in the DDDS Algorithm.
        /// It specifies the order in which NAPTR records with equal Order values should be processed, low numbers being processed before high numbers.
        /// This is similar to the preference field in an MX record, and is used so domain administrators can direct clients towards more capable hosts or lighter weight protocols.
        /// A client may look at records with higher preference values if it has a good reason to do so such as not supporting some protocol or service very well.
        /// The important difference between Order and Preference is that once a match is found the client must not consider records with a different Order but they may process records with the same Order but different Preferences.
        /// The only exception to this is noted in the second important Note in the DDDS algorithm specification concerning allowing clients to use more complex Service determination between steps 3 and 4 in the algorithm.
        /// Preference is used to give communicate a higher quality of service to rules that are considered the same from an authority standpoint but not from a simple load balancing standpoint.
        /// 
        /// It is important to note that DNS contains several load balancing mechanisms and if load balancing among otherwise equal services should be needed then methods such as SRV records or multiple A records should be utilized to accomplish load balancing.
        /// </summary>
        public ushort Preference { get; private set; }

        /// <summary>
        /// Flags to control aspects of the rewriting and interpretation of the fields in the record.
        /// Flags are single characters from the set A-Z and 0-9.
        /// The case of the alphabetic characters is not significant.
        /// The field can be empty.
        /// 
        /// It is up to the Application specifying how it is using this Database to define the Flags in this field.
        /// It must define which ones are terminal and which ones are not.
        /// </summary>
        public DataSegment Flags { get; private set; }

        /// <summary>
        /// Specifies the Service Parameters applicable to this this delegation path.
        /// It is up to the Application Specification to specify the values found in this field.
        /// </summary>
        public DataSegment Services { get; private set; }

        /// <summary>
        /// A substitution expression that is applied to the original string held by the client in order to construct the next domain name to lookup.
        /// See the DDDS Algorithm specification for the syntax of this field.
        /// 
        /// As stated in the DDDS algorithm, The regular expressions must not be used in a cumulative fashion, that is, they should only be applied to the original string held by the client, never to the domain name produced by a previous NAPTR rewrite.
        /// The latter is tempting in some applications but experience has shown such use to be extremely fault sensitive, very error prone, and extremely difficult to debug.
        /// </summary>
        public DataSegment Regexp { get; private set; }

        /// <summary>
        /// The next domain-name to query for depending on the potential values found in the flags field.
        /// This field is used when the regular expression is a simple replacement operation.
        /// Any value in this field must be a fully qualified domain-name.
        /// Name compression is not to be used for this field.
        /// 
        /// This field and the Regexp field together make up the Substitution Expression in the DDDS Algorithm.
        /// It is simply a historical optimization specifically for DNS compression that this field exists.
        /// The fields are also mutually exclusive.  
        /// If a record is returned that has values for both fields then it is considered to be in error and SHOULD be either ignored or an error returned.
        /// </summary>
        public DnsDomainName Replacement { get; private set; }

        public bool Equals(DnsResourceDataNamingAuthorityPointer other)
        {
            return other != null &&
                   Order.Equals(other.Order) &&
                   Preference.Equals(other.Preference) &&
                   Flags.Equals(other.Flags) &&
                   Services.Equals(other.Services) &&
                   Regexp.Equals(other.Regexp) &&
                   Replacement.Equals(other.Replacement);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataNamingAuthorityPointer);
        }

        internal DnsResourceDataNamingAuthorityPointer()
            : this(0, 0, DataSegment.Empty, DataSegment.Empty, DataSegment.Empty, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + GetStringLength(Flags) + GetStringLength(Services) + GetStringLength(Regexp) + Replacement.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Order, Order, Endianity.Big);
            buffer.Write(offset + Offset.Preference, Preference, Endianity.Big);
            offset += Offset.Flags;
            WriteString(buffer, ref offset, Flags);
            WriteString(buffer, ref offset, Services);
            WriteString(buffer, ref offset, Regexp);
            Replacement.WriteUncompressed(buffer, offset);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            ushort order = dns.ReadUShort(offsetInDns + Offset.Order, Endianity.Big);
            ushort preference = dns.ReadUShort(offsetInDns + Offset.Preference, Endianity.Big);

            DataSegment data = dns.SubSegment(offsetInDns + ConstantPartLength, length - ConstantPartLength);
            int dataOffset = 0;

            DataSegment flags = ReadString(data, ref dataOffset);
            if (flags == null || !IsLegalFlags(flags))
                return null;

            DataSegment services = ReadString(data, ref dataOffset);
            if (services == null)
                return null;

            DataSegment regexp = ReadString(data, ref dataOffset);
            if (regexp == null)
                return null;

            DnsDomainName replacement;
            int replacementLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns + ConstantPartLength + dataOffset, length - ConstantPartLength - dataOffset,
                                        out replacement, out replacementLength))
            {
                return null;
            }

            if (ConstantPartLength + dataOffset + replacementLength != length)
                return null;

            return new DnsResourceDataNamingAuthorityPointer(order, preference, flags, services, regexp, replacement);
        }

        private static bool IsLegalFlags(DataSegment flags)
        {
            return flags.All(flag => (flag >= '0' && flag <= '9' || flag >= 'A' && flag <= 'Z' || flag >= 'a' && flag <= 'z'));
        }
    }
}