using System;

namespace BPacket
{
    public struct MacAddress
    {
        public MacAddress(byte[] buffer, int offset)
        {
            _b1 = buffer[offset++];
            _b2 = buffer[offset++];
            _b3 = buffer[offset++];
            _b4 = buffer[offset++];
            _b5 = buffer[offset++];
            _b6 = buffer[offset];
        }

        public MacAddress(string address)
        {
            string[] hexes = address.Split(':');
            if (hexes.Length != 6)
                throw new ArgumentException("Failed parsing " + address + " as mac address. Expected 6 hexes and got " + hexes.Length + " hexes", "address");
            _b1 = Convert.ToByte(hexes[0], 16);
            _b2 = Convert.ToByte(hexes[1], 16);
            _b3 = Convert.ToByte(hexes[2], 16);
            _b4 = Convert.ToByte(hexes[3], 16);
            _b5 = Convert.ToByte(hexes[4], 16);
            _b6 = Convert.ToByte(hexes[5], 16);
        }

        public override string ToString()
        {
            return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}", _b1, _b2, _b3, _b4, _b5, _b6);
        }
        
        internal void Write(byte[] buffer, int offset)
        {
            buffer[offset++] = _b1;
            buffer[offset++] = _b2;
            buffer[offset++] = _b3;
            buffer[offset++] = _b4;
            buffer[offset++] = _b5;
            buffer[offset] = _b6;
        }

        private readonly byte _b1;
        private readonly byte _b2;
        private readonly byte _b3;
        private readonly byte _b4;
        private readonly byte _b5;
        private readonly byte _b6;
    }
}