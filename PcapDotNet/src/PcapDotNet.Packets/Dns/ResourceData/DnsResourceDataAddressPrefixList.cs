using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 3123.
    /// <pre>
    /// 0 Or more of:
    /// +-----+--------+---+-----------+
    /// | bit | 0-7    | 8 | 9-15      |
    /// +-----+--------+---+-----------+
    /// | 0   | ADDRESSFAMILY          |
    /// +-----+--------+---+-----------+
    /// | 16  | PREFIX | N | AFDLENGTH |
    /// +-----+--------+---+-----------+
    /// | 32  | AFDPART                |
    /// | ... |                        |
    /// +-----+------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Apl)]
    public sealed class DnsResourceDataAddressPrefixList: DnsResourceDataSimple, IEquatable<DnsResourceDataAddressPrefixList>
    {
        /// <summary>
        /// Constructs an instance from a sequence of DnsAddressPrefix.
        /// </summary>
        public DnsResourceDataAddressPrefixList(IList<DnsAddressPrefix> items)
        {
            Items = items.AsReadOnly();
        }

        /// <summary>
        /// Constructs an instance from a sequence of DnsAddressPrefix.
        /// </summary>
        public DnsResourceDataAddressPrefixList(params DnsAddressPrefix[] items)
            : this((IList<DnsAddressPrefix>)items)
        {
            Length = items.Sum(item => item.Length);
        }

        /// <summary>
        /// The DnsAddressPrefix items in the list.
        /// </summary>
        public ReadOnlyCollection<DnsAddressPrefix> Items { get; private set; }

        /// <summary>
        /// The number of bytes the resource data takes.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Two DnsResourceDataAddressPrefixList are equal iff their items are equal and in the same order.
        /// </summary>
        public bool Equals(DnsResourceDataAddressPrefixList other)
        {
            return other != null &&
                   Items.SequenceEqual(other.Items);
        }

        /// <summary>
        /// Two DnsResourceDataAddressPrefixList are equal iff their items are equal and in the same order.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataAddressPrefixList);
        }

        /// <summary>
        /// A hash code based on the items.
        /// </summary>
        public override int GetHashCode()
        {
            return Items.SequenceGetHashCode();
        }

        internal DnsResourceDataAddressPrefixList()
            : this(new DnsAddressPrefix[0])
        {
        }

        internal override int GetLength()
        {
            return Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            foreach (DnsAddressPrefix item in Items)
                item.Write(buffer, ref offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DnsAddressPrefix> items = new List<DnsAddressPrefix>();
            while (data.Length != 0)
            {
                DnsAddressPrefix item = DnsAddressPrefix.Read(data);
                if (item == null)
                    return null;
                items.Add(item);
                data = data.Subsegment(item.Length, data.Length - item.Length);
            }

            return new DnsResourceDataAddressPrefixList(items);
        }
    }
}