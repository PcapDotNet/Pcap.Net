using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Faltstrom.
    /// <pre>
    /// +-----+----------+--------+
    /// | bit | 0-15     | 16-31  |
    /// +-----+----------+--------+
    /// | 0   | Priority | Weight |
    /// +-----+----------+--------+
    /// | 32  | Target            |
    /// | ... |                   |
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Uri)]
    public sealed class DnsResourceDataUri : DnsResourceDataSimple, IEquatable<DnsResourceDataUri>
    {
        private static class Offset
        {
            public const int Priority = 0;
            public const int Weight = Priority + sizeof(ushort);
            public const int Target = Weight + sizeof(ushort);
        }

        private const int ConstantPartLength = Offset.Target;

        public DnsResourceDataUri(ushort priority, ushort weight, IList<DataSegment> target)
        {
            Priority = priority;
            Weight = weight;
            Target = target.AsReadOnly();
        }

        /// <summary>
        /// The priority of the target URI in this RR.
        /// A client must attempt to contact the URI with the lowest-numbered priority it can reach;
        /// URIs with the same priority should be tried in the order defined by the weight field.
        /// </summary>
        public ushort Priority { get; private set; }

        /// <summary>
        /// A server selection mechanism.
        /// The weight field specifies a relative weight for entries with the same priority.
        /// Larger weights should be given a proportionately higher probability of being selected.
        /// </summary>
        public ushort Weight { get; private set; }

        /// <summary>
        /// The URI of the target.
        /// Resolution of the URI is according to the definitions for the Scheme of the URI.
        /// </summary>
        public ReadOnlyCollection<DataSegment> Target { get; private set; }

        public bool Equals(DnsResourceDataUri other)
        {
            return other != null &&
                   Priority.Equals(other.Priority) &&
                   Weight.Equals(other.Weight) &&
                   Target.SequenceEqual(other.Target);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataUri);
        }

        public override int GetHashCode()
        {
            return BitSequence.Merge(Priority, Weight).GetHashCode() ^ Target.SequenceGetHashCode();
        }

        internal DnsResourceDataUri()
            : this(0, 0, new DataSegment[0])
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Target.Sum(targetPart => GetStringLength(targetPart));
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Priority, Priority, Endianity.Big);
            buffer.Write(offset + Offset.Weight, Weight, Endianity.Big);
            int targetOffset = offset + Offset.Target;
            foreach (DataSegment targetPart in Target)
                WriteString(buffer, ref targetOffset, targetPart);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            ushort priority = data.ReadUShort(Offset.Priority, Endianity.Big);
            ushort weight = data.ReadUShort(Offset.Weight, Endianity.Big);

            List<DataSegment> target = new List<DataSegment>();
            int targetDataOffset = Offset.Target;
            while (data.Length > targetDataOffset)
            {
                DataSegment targetPart = ReadString(data, ref targetDataOffset);
                if (targetPart == null)
                    return null;
                target.Add(targetPart);
            }
            return new DnsResourceDataUri(priority, weight, target);
        }
    }
}