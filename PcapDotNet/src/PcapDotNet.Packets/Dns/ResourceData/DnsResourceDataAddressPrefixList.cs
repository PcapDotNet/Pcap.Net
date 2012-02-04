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
        public DnsResourceDataAddressPrefixList(IList<DnsAddressPrefix> items)
        {
            Items = items.AsReadOnly();
        }

        public DnsResourceDataAddressPrefixList(params DnsAddressPrefix[] items)
            : this((IList<DnsAddressPrefix>)items)
        {
            Length = items.Sum(item => item.Length);
        }

        public ReadOnlyCollection<DnsAddressPrefix> Items { get; private set; }
        public int Length { get; private set; }

        public bool Equals(DnsResourceDataAddressPrefixList other)
        {
            return other != null &&
                   Items.SequenceEqual(other.Items);
        }

        public override bool Equals(object other)
        {
            return Equals(other as DnsResourceDataAddressPrefixList);
        }

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
                data = data.SubSegment(item.Length, data.Length - item.Length);
            }

            return new DnsResourceDataAddressPrefixList(items);
        }
    }
}