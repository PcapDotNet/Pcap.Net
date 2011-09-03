using System;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomPacketsExtensions
    {
        public static Datagram NextDatagram(this Random random, int length)
        {
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return new Datagram(buffer);
        }

        public static DataLinkKind NextDataLinkKind(this Random random)
        {
            return random.NextEnum<DataLinkKind>();
        }

        public static Packet NextPacket(this Random random, int length)
        {
            byte[] buffer = new byte[length];
            random.NextBytes(buffer);
            return new Packet(buffer, DateTime.Now, random.NextDataLinkKind());
        }

        public static PayloadLayer NextPayloadLayer(this Random random, int length)
        {
            return new PayloadLayer
                       {
                           Data = random.NextDatagram(length)
                       };
        }

        public static DataSegment NextDataSegment(this Random random, int length)
        {
            return new DataSegment(random.NextBytes(length));
        }
    }
}
