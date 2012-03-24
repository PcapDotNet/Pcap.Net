﻿using System;
using PcapDotNet.Base;

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

        private const int ConstantPartLength = Offset.Data;

        /// <summary>
        /// Construct an instance out of the coding subcoding and data fields.
        /// </summary>
        /// <param name="codingSubCoding">
        /// A combination of coding and subcoding.
        /// Has a valid enum value if the subcoding is defined specifically for the coding.
        /// </param>
        /// <param name="data">Variable length and could be null in some cases.</param>
        public DnsResourceDataSink(DnsSinkCodingSubCoding codingSubCoding, DataSegment data)
            : this((DnsSinkCoding)((ushort)codingSubCoding >> 8), (byte)((ushort)codingSubCoding & 0x00FF), data)
        {
        }

        /// <summary>
        /// Constructs an instance out of the coding, subcoding and data fields.
        /// </summary>
        /// <param name="coding">Gives the general structure of the data.</param>
        /// <param name="subCoding">Provides additional information depending on the value of the coding.</param>
        /// <param name="data">Variable length and could be null in some cases.</param>
        public DnsResourceDataSink(DnsSinkCoding coding, byte subCoding, DataSegment data)
        {
            Coding = coding;
            SubCoding = subCoding;
            Data = data;
        }

        /// <summary>
        /// Gives the general structure of the data.
        /// </summary>
        public DnsSinkCoding Coding { get; private set; }

        /// <summary>
        /// Provides additional information depending on the value of the coding.
        /// </summary>
        public byte SubCoding { get; private set; }

        /// <summary>
        /// Returns a combination of coding and subcoding.
        /// Has a valid enum value if the subcoding is defined specifically for the coding.
        /// </summary>
        public DnsSinkCodingSubCoding CodingSubCoding
        {
            get
            {
                ushort codingSubcoding = BitSequence.Merge((byte)Coding, SubCoding);
                return (DnsSinkCodingSubCoding)codingSubcoding;
            }
        }

        /// <summary>
        /// Variable length and could be null in some cases.
        /// </summary>
        public DataSegment Data { get; private set; }

        /// <summary>
        /// Two are equal iff their coding, subcoding and data fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataSink other)
        {
            return other != null &&
                   Coding.Equals(other.Coding) &&
                   SubCoding.Equals(other.SubCoding) &&
                   Data.Equals(other.Data);
        }

        /// <summary>
        /// Two are equal iff their coding, subcoding and data fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataSink);
        }

        /// <summary>
        /// A hash code based on the coding, subcoding and data fields.
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(CodingSubCoding, Data);
        }

        internal DnsResourceDataSink()
            : this(DnsSinkCodingSubCoding.Asn1SnmpBasicEncodingRules, DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + Data.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Coding, (byte)Coding);
            buffer.Write(offset + Offset.Subcoding, SubCoding);
            Data.Write(buffer, offset + Offset.Data);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            DnsSinkCoding coding = (DnsSinkCoding)data[Offset.Coding];
            byte subcoding = data[Offset.Subcoding];
            DataSegment dataValue = data.Subsegment(Offset.Data, data.Length - ConstantPartLength);

            return new DnsResourceDataSink(coding, subcoding, dataValue);
        }
    }
}