using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2782.
    /// <pre>
    /// +-----+----------+
    /// | bit | 0-15     |
    /// +-----+----------+
    /// | 0   | Priority |
    /// +-----+----------+
    /// | 16  | Weight   |
    /// +-----+----------+
    /// | 32  | Port     |
    /// +-----+----------+
    /// | 48  | Target   |
    /// | ... |          |
    /// +-----+----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Srv)]
    public sealed class DnsResourceDataServerSelection : DnsResourceDataNoCompression, IEquatable<DnsResourceDataServerSelection>
    {
        private static class Offset
        {
            public const int Priority = 0;
            public const int Weight = Priority + sizeof(ushort);
            public const int Port = Weight + sizeof(ushort);
            public const int Target = Port + sizeof(ushort);
        }

        public const int ConstantPartLength = Offset.Target;

        public DnsResourceDataServerSelection(ushort priority, ushort weight, ushort port, DnsDomainName target)
        {
            Priority = priority;
            Weight = weight;
            Port = port;
            Target = target;
        }

        /// <summary>
        /// The priority of this target host.
        /// A client must attempt to contact the target host with the lowest-numbered priority it can reach; 
        /// target hosts with the same priority should be tried in an order defined by the weight field.
        /// </summary>
        public ushort Priority { get; private set; }

        /// <summary>
        /// A server selection mechanism.
        /// The weight field specifies a relative weight for entries with the same priority.
        /// Larger weights should be given a proportionately higher probability of being selected.
        /// Domain administrators should use Weight 0 when there isn't any server selection to do, to make the RR easier to read for humans (less noisy).
        /// In the presence of records containing weights greater than 0, records with weight 0 should have a very small chance of being selected.
        /// 
        /// In the absence of a protocol whose specification calls for the use of other weighting information, a client arranges the SRV RRs of the same Priority in the order in which target hosts,
        /// specified by the SRV RRs, will be contacted. 
        /// The following algorithm SHOULD be used to order the SRV RRs of the same priority:
        /// To select a target to be contacted next, arrange all SRV RRs (that have not been ordered yet) in any order, except that all those with weight 0 are placed at the beginning of the list.
        /// Compute the sum of the weights of those RRs, and with each RR associate the running sum in the selected order.
        /// Then choose a uniform random number between 0 and the sum computed (inclusive), and select the RR whose running sum value is the first in the selected order which is greater than or equal to the random number selected.
        /// The target host specified in the selected SRV RR is the next one to be contacted by the client.
        /// Remove this SRV RR from the set of the unordered SRV RRs and apply the described algorithm to the unordered SRV RRs to select the next target host.
        /// Continue the ordering process until there are no unordered SRV RRs.
        /// This process is repeated for each Priority.
        /// </summary>
        public ushort Weight { get; private set; }

        /// <summary>
        /// The port on this target host of this service. 
        /// This is often as specified in Assigned Numbers but need not be.
        /// </summary>
        public ushort Port { get; private set; }

        /// <summary>
        /// The domain name of the target host.
        /// There must be one or more address records for this name, the name must not be an alias (in the sense of RFC 1034 or RFC 2181).
        /// Implementors are urged, but not required, to return the address record(s) in the Additional Data section.
        /// Unless and until permitted by future standards action, name compression is not to be used for this field.
        /// 
        /// A Target of "." means that the service is decidedly not available at this domain.
        /// </summary>
        public DnsDomainName Target { get; private set; }

        public bool Equals(DnsResourceDataServerSelection other)
        {
            return other != null &&
                   Priority.Equals(other.Priority) &&
                   Weight.Equals(other.Weight) &&
                   Port.Equals(other.Port) &&
                   Target.Equals(other.Target);
        }

        public override int GetHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(Priority, Weight), Port, Target);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataServerSelection);
        }

        internal DnsResourceDataServerSelection()
            : this(0, 0, 0, DnsDomainName.Root)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Target.NonCompressedLength;
        }

        internal override int WriteData(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Priority, Priority, Endianity.Big);
            buffer.Write(offset + Offset.Weight, Weight, Endianity.Big);
            buffer.Write(offset + Offset.Port, Port, Endianity.Big);
            Target.WriteUncompressed(buffer, offset + Offset.Target);

            return GetLength();
        }

        internal override DnsResourceData CreateInstance(DnsDatagram dns, int offsetInDns, int length)
        {
            if (length < ConstantPartLength)
                return null;

            ushort priortiy = dns.ReadUShort(offsetInDns + Offset.Priority, Endianity.Big);
            ushort weight = dns.ReadUShort(offsetInDns + Offset.Weight, Endianity.Big);
            ushort port = dns.ReadUShort(offsetInDns + Offset.Port, Endianity.Big);

            DnsDomainName target;
            int targetLength;
            if (!DnsDomainName.TryParse(dns, offsetInDns + Offset.Target, length - ConstantPartLength, out target, out targetLength))
                return null;

            if (ConstantPartLength + targetLength != length)
                return null;

            return new DnsResourceDataServerSelection(priortiy, weight, port, target);
        }
    }
}