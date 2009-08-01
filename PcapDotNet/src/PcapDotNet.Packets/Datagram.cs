using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets
{
    public class Datagram : IEquatable<Datagram>, IEnumerable<byte>
    {
        public Datagram(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        {
        }

        public Datagram(byte[] buffer, int offset, int length)
        {
            _buffer = buffer;
            _startOffset = offset;
            _length = length;
        }

        public static Datagram Empty
        {
            get { return _empty; }
        }

        public int Length
        {
            get { return _length; }
        }

        public byte this[int offset]
        {
            get { return _buffer[StartOffset + offset]; }
        }

        public bool IsValid
        {
            get
            {
                if (_isValid == null)
                    _isValid = CalculateIsValid();
                return _isValid.Value;
            }
        }

        public virtual bool CalculateIsValid()
        {
            return true;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i != Length; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(Datagram other)
        {
            if (Length != other.Length)
                return false;

            for (int i = 0; i != Length; ++i)
            {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Datagram);
        }

        public override int GetHashCode()
        {
            int i = 0;
            return Length.GetHashCode() ^
                   this.Aggregate(0, (value, b) => value ^ (b << (8 * (i++ % 4))));
        }

        internal void Write(byte[] buffer, int offset)
        {
            System.Buffer.BlockCopy(_buffer, StartOffset, buffer, offset, Length);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        protected byte[] Buffer
        {
            get { return _buffer; }
        }

        protected int StartOffset
        {
            get { return _startOffset; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ushort")]
        protected ushort ReadUShort(int offset, Endianity endianity)
        {
            return Buffer.ReadUShort(StartOffset + offset, endianity);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        protected uint ReadUInt(int offset, Endianity endianity)
        {
            return Buffer.ReadUInt(StartOffset + offset, endianity);
        }

        protected MacAddress ReadMacAddress(int offset, Endianity endianity)
        {
            return Buffer.ReadMacAddress(StartOffset + offset, endianity);
        }

        private static readonly Datagram _empty = new Datagram(new byte[0], 0, 0);
        private readonly byte[] _buffer;
        private readonly int _startOffset;
        private readonly int _length;
        private bool? _isValid;
    }
}