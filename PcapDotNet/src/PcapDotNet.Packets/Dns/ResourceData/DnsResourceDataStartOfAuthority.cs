using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// <pre>
    /// +-------+---------+
    /// | bit   | 0-31    |
    /// +-------+---------+
    /// | 0     | MNAME   |
    /// | ...   |         |
    /// +-------+---------+
    /// | X     | RNAME   |
    /// | ...   |         |
    /// +-------+---------+
    /// | Y     | SERIAL  |
    /// +-------+---------+
    /// | Y+32  | REFRESH |
    /// +-------+---------+
    /// | Y+64  | RETRY   |
    /// +-------+---------+
    /// | Y+96  | EXPIRE  |
    /// +-------+---------+
    /// | Y+128 | MINIMUM |
    /// +-------+---------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.StartOfAuthority)]
    public sealed class DnsResourceDataStartOfAuthority : DnsResourceData, IEquatable<DnsResourceDataStartOfAuthority>
    {
        private static class Offset
        {
            public const int Serial = 0;
            public const int Refresh = Serial + SerialNumber32.SizeOf;
            public const int Retry = Refresh + sizeof(uint);
            public const int Expire = Retry + sizeof(uint);
            public const int MinimumTtl = Expire + sizeof(uint);
        }

        private const int ConstantPartLength = Offset.MinimumTtl + sizeof(uint);

        /// <summary>
        /// Constructs an instance out of the main name server, responsible mailbox, serial, refresh, retry, expire and minimum TTL fields.
        /// </summary>
        /// <param name="mainNameServer">The domain-name of the name server that was the original or primary source of data for this zone.</param>
        /// <param name="responsibleMailbox">A domain-name which specifies the mailbox of the person responsible for this zone.</param>
        /// <param name="serial">
        /// The unsigned 32 bit version number of the original copy of the zone.
        /// Zone transfers preserve this value.
        /// This value wraps and should be compared using sequence space arithmetic.
        /// </param>
        /// <param name="refresh">A 32 bit time interval before the zone should be refreshed.</param>
        /// <param name="retry">A 32 bit time interval that should elapse before a failed refresh should be retried.</param>
        /// <param name="expire">
        /// A 32 bit time value that specifies the upper limit on the time interval that can elapse before the zone is no longer authoritative.
        /// </param>
        /// <param name="minimumTtl">The unsigned 32 bit minimum TTL field that should be exported with any RR from this zone.</param>
        public DnsResourceDataStartOfAuthority(DnsDomainName mainNameServer, DnsDomainName responsibleMailbox,
                                               SerialNumber32 serial, uint refresh, uint retry, uint expire, uint minimumTtl)
        {
            MainNameServer = mainNameServer;
            ResponsibleMailbox = responsibleMailbox;
            Serial = serial;
            Refresh = refresh;
            Retry = retry;
            Expire = expire;
            MinimumTtl = minimumTtl;
        }

        /// <summary>
        /// The domain-name of the name server that was the original or primary source of data for this zone.
        /// </summary>
        public DnsDomainName MainNameServer { get; private set; }

        /// <summary>
        /// A domain-name which specifies the mailbox of the person responsible for this zone.
        /// </summary>
        public DnsDomainName ResponsibleMailbox { get; private set; }

        /// <summary>
        /// The unsigned 32 bit version number of the original copy of the zone.
        /// Zone transfers preserve this value.
        /// This value wraps and should be compared using sequence space arithmetic.
        /// </summary>
        public SerialNumber32 Serial { get; private set; }

        /// <summary>
        /// A 32 bit time interval before the zone should be refreshed.
        /// </summary>
        public uint Refresh { get; private set; }

        /// <summary>
        /// A 32 bit time interval that should elapse before a failed refresh should be retried.
        /// </summary>
        public uint Retry { get; private set; }

        /// <summary>
        /// A 32 bit time value that specifies the upper limit on the time interval that can elapse before the zone is no longer authoritative.
        /// </summary>
        public uint Expire { get; private set; }

        /// <summary>
        /// The unsigned 32 bit minimum TTL field that should be exported with any RR from this zone.
        /// </summary>
        public uint MinimumTtl { get; private set; }

        /// <summary>
        /// Two DnsResourceDataStartOfAuthority are equal iff their main name server, responsible mailbox, serial, refresh, retry, expire 
        /// and minimum TTL fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataStartOfAuthority other)
        {
            return other != null &&
                   MainNameServer.Equals(other.MainNameServer) &&
                   ResponsibleMailbox.Equals(other.ResponsibleMailbox) &&
                   Serial.Equals(other.Serial) &&
                   Refresh.Equals(other.Refresh) &&
                   Retry.Equals(other.Retry) &&
                   Expire.Equals(other.Expire) &&
                   MinimumTtl.Equals(other.MinimumTtl);
        }

        /// <summary>
        /// Two DnsResourceDataStartOfAuthority are equal iff their main name server, responsible mailbox, serial, refresh, retry, expire 
        /// and minimum TTL fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataStartOfAuthority);
        }

        /// <summary>
        /// A hash code based on the main name server, responsible mailbox, serial, refresh, retry, expire and minimum TTL fields
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(MainNameServer, ResponsibleMailbox, Serial, Refresh, Retry, Expire, MinimumTtl);
        }

        internal DnsResourceDataStartOfAuthority()
            : this(DnsDomainName.Root, DnsDomainName.Root, 0, 0, 0, 0, 0)
        {
        }

        internal override int GetLength(DnsDomainNameCompressionData compressionData, int offsetInDns)
        {
            return MainNameServer.GetLength(compressionData, offsetInDns) +
                   ResponsibleMailbox.GetLength(compressionData, offsetInDns) +
                   ConstantPartLength;
        }

        internal override int WriteData(byte[] buffer, int dnsOffset, int offsetInDns, DnsDomainNameCompressionData compressionData)
        {
            int numBytesWritten = MainNameServer.Write(buffer, dnsOffset, compressionData, offsetInDns);
            numBytesWritten += ResponsibleMailbox.Write(buffer, dnsOffset, compressionData, offsetInDns + numBytesWritten);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Serial, Serial.Value, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Refresh, Refresh, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Retry, Retry, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.Expire, Expire, Endianity.Big);
            buffer.Write(dnsOffset + offsetInDns + numBytesWritten + Offset.MinimumTtl, MinimumTtl, Endianity.Big);

            return numBytesWritten + ConstantPartLength;
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            DnsDomainName mainNameServer;
            int domainNameLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out mainNameServer, out domainNameLength))
                return null;
            offsetInDns += domainNameLength;
            length -= domainNameLength;

            DnsDomainName responsibleMailbox;
            if (!DnsDomainName.TryParse(dns, offsetInDns, length, out responsibleMailbox, out domainNameLength))
                return null;
            offsetInDns += domainNameLength;
            length -= domainNameLength;

            if (length != ConstantPartLength)
                return null;

            uint serial = dns.ReadUInt(offsetInDns + Offset.Serial, Endianity.Big); ;
            uint refresh = dns.ReadUInt(offsetInDns + Offset.Refresh, Endianity.Big); ;
            uint retry = dns.ReadUInt(offsetInDns + Offset.Retry, Endianity.Big); ;
            uint expire = dns.ReadUInt(offsetInDns + Offset.Expire, Endianity.Big); ;
            uint minimumTtl = dns.ReadUInt(offsetInDns + Offset.MinimumTtl, Endianity.Big); ;

            return new DnsResourceDataStartOfAuthority(mainNameServer, responsibleMailbox, serial, refresh, retry, expire, minimumTtl);
        }
    }
}