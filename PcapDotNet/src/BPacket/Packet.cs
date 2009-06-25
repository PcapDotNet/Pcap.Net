using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPacket
{
    public class Packet
    {
        public Packet(byte[] data, DateTime timestamp)
        {
            _data = data;
            _timestamp = timestamp;
        }

        public int Length
        {
            get { return _data.Length; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public byte[] Buffer
        {
            get { return _data; }
        }

        private readonly byte[] _data;
        private readonly DateTime _timestamp;
    }
}
