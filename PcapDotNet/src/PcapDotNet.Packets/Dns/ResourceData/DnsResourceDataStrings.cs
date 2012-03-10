using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Base class for any resource data that contains DNS strings.
    /// Each DNS string is a segment of up to 255 bytes.
    /// The format of each DNS string is one byte for the length of the string and then the specified number of bytes.
    /// </summary>
    public abstract class DnsResourceDataStrings : DnsResourceDataSimple, IEquatable<DnsResourceDataStrings>
    {
        /// <summary>
        /// Two strings resources datas are equal if they are of the same concrete type and their strings are equal and in the same order.
        /// </summary>
        public bool Equals(DnsResourceDataStrings other)
        {
            return other != null &&
                   GetType() == other.GetType() &&
                   Strings.SequenceEqual(other.Strings);
        }

        /// <summary>
        /// Two strings resources datas are equal if they are of the same concrete type and their strings are equal and in the same order.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataStrings);
        }

        /// <summary>
        /// A hash code based on the concrete type and the strings.
        /// </summary>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ Strings.SequenceGetHashCode();
        }

        internal DnsResourceDataStrings(ReadOnlyCollection<DataSegment> strings)
        {
            Strings = strings;
        }

        internal DnsResourceDataStrings(params DataSegment[] strings)
            : this(strings.AsReadOnly())
        {
        }

        internal ReadOnlyCollection<DataSegment> Strings { get; private set; }

        internal sealed override int GetLength()
        {
            return Strings.Sum(str => sizeof(byte) + str.Length);
        }

        internal sealed override void WriteDataSimple(byte[] buffer, int offset)
        {
            foreach (DataSegment str in Strings)
            {
                buffer.Write(ref offset, (byte)str.Length);
                str.Write(buffer, ref offset);
            }
        }

        internal static List<DataSegment> ReadStrings(DataSegment data, int numExpected = 0)
        {
            List<DataSegment> strings = new List<DataSegment>(numExpected);
            int offset = 0;
            while (offset != data.Length)
            {
                DataSegment str = ReadString(data, ref offset);
                if (str == null)
                    return null;
                strings.Add(str);
            }

            return strings;
        }
    }
}