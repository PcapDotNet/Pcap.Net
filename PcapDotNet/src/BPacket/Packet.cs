using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packets
{
    public class Packet
    {
        public Packet(byte[] data, DateTime timestamp, IDataLink dataLink)
        {
            _data = data;
            _timestamp = timestamp;
            _dataLink = dataLink;
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

        private readonly byte[] _data;
        private readonly DateTime _timestamp;
        private readonly IDataLink _dataLink;
    }
}
