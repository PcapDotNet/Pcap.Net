namespace Packets
{
    /// <summary>
    /// +------+-----------------+------------+------------------+
    /// | Byte | 0-5             | 6-11       | 12-13            |
    /// +------+-----------------+------------+------------------+
    /// | 0    | MAC Destination | MAC Source | EtherType/Length |
    /// +------+-----------------+------------+------------------+
    /// | 14   | Data                                            |
    /// +------+-------------------------------------------------+
    /// </summary>
    public class EthernetDatagram : Datagram
    {
        private static class Offset
        {
            public const int Destination = 0;
            public const int Source = 6;
            public const int EtherTypeLength = 12;
        }

        public const int HeaderLength = 14;

        internal EthernetDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        public MacAddress Source
        { 
            get
            {
                return new MacAddress(Buffer, StartOffset + Offset.Source);
            }
        }

        public MacAddress Destination
        { 
            get
            {
                return new MacAddress(Buffer, StartOffset + Offset.Destination);
            }
        }

        public EthernetType EtherType 
        {
            get
            {
                return (EthernetType)ReadUShort(Offset.EtherTypeLength, Endianity.Big);
            }
        }

        public override bool CalculateIsValid()
        {
            if (Length < HeaderLength)
                return false;

            switch (EtherType)
            {
                case EthernetType.Arp:
                case EthernetType.Ieee8021Q:
                case EthernetType.IpV4:
                case EthernetType.IpV6:
                    return true;

                default:
                    return false;
            }
        }

        internal static void WriteHeader(byte[] buffer, int offset, MacAddress ethernetSource, MacAddress ethernetDestination, EthernetType ethernetType)
        {
            ethernetSource.Write(buffer, offset + Offset.Source);
            ethernetDestination.Write(buffer, offset + Offset.Destination);
            ethernetDestination.Write(buffer, offset + Offset.Destination);
            buffer.Write(offset + Offset.EtherTypeLength, (ushort)ethernetType, Endianity.Big);
        }
    }
}