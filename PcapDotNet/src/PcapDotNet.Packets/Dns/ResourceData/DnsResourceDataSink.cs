using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Eastlake.
    /// <pre>
    /// +-----+--------+-----------+
    /// | bit | 0-7    | 8-15      |
    /// +-----+--------+-----------+
    /// | 0   | coding | subcoding |
    /// +-----+--------+-----------+
    /// | 16  | data               |
    /// | ... |                    |
    /// +-----+--------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Sink)]
    public sealed class DnsResourceDataSink: DnsResourceDataSimple, IEquatable<DnsResourceDataSink>
    {
        private static class Offset
        {
            public const int Coding = 0;
            public const int Subcoding = Coding + sizeof(byte);
            public const int Data = Subcoding + sizeof(byte);
        }

        public const int ConstantPartLength = Offset.Data;

        public DnsResourceDataSink(DnsSinkCodingSubcoding codingSubcoding, DataSegment data)
            : this((DnsSinkCoding)((ushort)codingSubcoding >> 8), (byte)((ushort)codingSubcoding & 0x00FF), data)
        {
        }

        public DnsResourceDataSink(DnsSinkCoding coding, byte subcoding, DataSegment data)
        {
            Coding = coding;
            Subcoding = subcoding;
            Data = data;
        }

        /// <summary>
        /// Gives the general structure of the data.
        /// </summary>
        public DnsSinkCoding Coding { get; private set; }

        /// <summary>
        /// Provides additional information depending on the value of the coding.
        /// </summary>
        public byte Subcoding { get; private set; }

        /// <summary>
        /// Returns a combination of coding and subcoding.
        /// Has a valid enum value if the subcoding is defined specifically for the coding.
        /// </summary>
        public DnsSinkCodingSubcoding CodingSubcoding
        {
            get
            {
                ushort codingSubcoding = (ushort)(((ushort)Coding << 8) | Subcoding);
                return (DnsSinkCodingSubcoding)codingSubcoding;
            }
        }

        /// <summary>
        /// Variable length and could be null in some cases.
        /// </summary>
        public DataSegment Data { get; private set; }

        public bool Equals(DnsResourceDataSink other)
        {
            return other != null &&
                   Coding.Equals(other.Coding) &&
                   Subcoding.Equals(other.Subcoding) &&
                   Data.Equals(other.Data);
        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataSink);
        }

        internal DnsResourceDataSink()
            : this(DnsSinkCodingSubcoding.Asn1SnmpBer, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Data.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Coding, (byte)Coding);
            buffer.Write(offset + Offset.Subcoding, Subcoding);
            Data.Write(buffer, offset + Offset.Data);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsSinkCoding coding = (DnsSinkCoding)data[Offset.Coding];
            byte subcoding = data[Offset.Subcoding];
            DataSegment dataValue = data.SubSegment(Offset.Data, data.Length - ConstantPartLength);

            return new DnsResourceDataSink(coding, subcoding, dataValue);
        }
    }
}