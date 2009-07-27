using System;
using System.Collections.Generic;
using System.Linq;

namespace Packets
{
    public class Packet : IEquatable<Packet>
    {
        public Packet(byte[] data, DateTime timestamp, IDataLink dataLink)
        {
            _data = data;
            _timestamp = timestamp;
            _dataLink = dataLink;
        }

        public byte this[int offset]
        {
            get { return _data[offset]; }
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public IDataLink DataLink
        {
            get { return _dataLink; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Buffer
        {
            get { return _data; }
        }

        public bool Equals(Packet other)
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
            return Equals(obj as Packet);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            int offset = 0;
            for (; offset < _data.Length - 3; offset += 4)
                hashCode ^= _data.ReadInt(offset, Endianity.Small);
            if (offset < _data.Length - 1)
                hashCode ^= _data.ReadShort(offset, Endianity.Small);
            if (offset < _data.Length)
                hashCode ^= _data[offset] >> 2;
            return hashCode;
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

        public EthernetDatagram Ethernet
        {
            get
            {
                if (_ethernet == null)
                    _ethernet = new EthernetDatagram(Buffer, 0, Length);
                return _ethernet;
            }
        }

        private bool CalculateIsValid()
        {
            switch (DataLink.Kind)
            {
                case DataLinkKind.Ethernet:
                    return Ethernet.IsValid;
                default:
                    return false;
            }
        }

        private readonly byte[] _data;
        private readonly DateTime _timestamp;
        private readonly IDataLink _dataLink;
        private bool? _isValid;

        private EthernetDatagram _ethernet;
    }
}
