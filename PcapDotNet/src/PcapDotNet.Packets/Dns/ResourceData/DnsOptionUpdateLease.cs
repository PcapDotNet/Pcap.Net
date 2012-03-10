namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// http://files.dns-sd.org/draft-sekar-dns-ul.txt.
    /// <pre>
    /// +-----+-------+
    /// | bit | 0-31  |
    /// +-----+-------+
    /// | 0   | LEASE |
    /// +-----+-------+
    /// </pre>
    /// </summary>
    public sealed class DnsOptionUpdateLease : DnsOption
    {
        /// <summary>
        /// The number of bytes this option data can take.
        /// </summary>
        public const int ConstDataLength = sizeof(int);

        /// <summary>
        /// Builds 
        /// </summary>
        /// <param name="lease"></param>
        public DnsOptionUpdateLease(int lease)
            : base(DnsOptionCode.UpdateLease)
        {
            Lease = lease;
        }

        /// <summary>
        /// Indicating the lease life, in seconds, desired by the client.
        /// In Update Responses, this field contains the actual lease granted by the server.
        /// Note that the lease granted by the server may be less than, greater than, or equal to the value requested by the client.
        /// To reduce network and server load, a minimum lease of 30 minutes (1800 seconds) is recommended.
        /// Note that leases are expected to be sufficiently long as to make timer discrepancies (due to transmission latency, etc.)
        /// between a client and server negligible.
        /// Clients that expect the updated records to be relatively static may request appropriately longer leases.
        /// Servers may grant relatively longer or shorter leases to reduce network traffic due to refreshes, or reduce stale data, respectively.
        /// </summary>
        public int Lease { get; private set; }

        public override int DataLength
        {
            get { return ConstDataLength; }
        }

        internal override bool EqualsData(DnsOption other)
        {
            return Lease.Equals(((DnsOptionUpdateLease)other).Lease);
        }

        internal override int DataGetHashCode()
        {
            return Lease.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Lease, Endianity.Big);
        }

        internal static DnsOptionUpdateLease Read(DataSegment data)
        {
            if (data.Length < ConstDataLength)
                return null;

            int lease = data.ReadInt(0, Endianity.Big);

            return new DnsOptionUpdateLease(lease);
        }
    }
}