using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private readonly byte[] _data;
        private readonly DateTime _timestamp;
        private readonly IDataLink _dataLink;
    }
}
