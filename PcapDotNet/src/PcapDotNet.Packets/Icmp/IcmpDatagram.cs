using System;

namespace PcapDotNet.Packets.Icmp
{
    public class IcmpDatagram : Datagram
    {
        /// <summary>
        /// The number of bytes the ICMP header takes.
        /// </summary>
        public const int HeaderLength = 8;

        private static class Offset
        {
            public const int Type = 0;
            public const int Code = 1;
            public const int Checksum = 2;
            public const int Variable = 4;
        }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public IcmpType Type
        {
            get { return (IcmpType)this[Offset.Type]; }
        }

        public byte Code
        {
            get { return this[Offset.Code]; }
        }

        public IcmpTypeAndCode TypeAndCode
        {
            get { return (IcmpTypeAndCode)ReadUShort(Offset.Type, Endianity.Big); }
        }

        /// <summary>
        /// The checksum is the 16-bit ones's complement of the one's complement sum of the ICMP message starting with the ICMP Type.
        /// For computing the checksum, the checksum field should be zero.
        /// This checksum may be replaced in the future.
        /// </summary>
        public ushort Checksum
        {
            get { return ReadUShort(Offset.Checksum, Endianity.Big); }
        }

        /// <summary>
        /// True iff the checksum value is correct according to the datagram data.
        /// </summary>
        public bool IsChecksumCorrect
        {
            get
            {
                if (_isChecksumCorrect == null)
                    _isChecksumCorrect = (CalculateChecksum() == Checksum);
                return _isChecksumCorrect.Value;
            }
        }

        public uint Variable
        {
            get { return ReadUInt(Offset.Variable, Endianity.Big); }
        }

        public Datagram Payload
        {
            get { return new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength); }
        }

        internal IcmpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        protected override bool CalculateIsValid()
        {
            if (Length < HeaderLength || !IsChecksumCorrect)
                return false;

            switch (Type)
            {
                default:
                    return false;
            }
        }

        private ushort CalculateChecksum()
        {
            uint sum = Sum16Bits(Buffer, StartOffset, Math.Min(Offset.Checksum, Length)) +
                       Sum16Bits(Buffer, StartOffset + Offset.Checksum + sizeof(ushort), Length - Offset.Checksum - sizeof(ushort));

            return Sum16BitsToChecksum(sum);
        }

        private bool? _isChecksumCorrect;
    }
}