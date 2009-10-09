namespace PcapDotNet.Packets.Icmp
{
    public class IcmpTypedDatagram : Datagram
    {
        public const int HeaderLength = 4;

        internal IcmpTypedDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}