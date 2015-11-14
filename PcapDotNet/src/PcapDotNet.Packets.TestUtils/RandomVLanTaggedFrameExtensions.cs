using System;
using System.Diagnostics.CodeAnalysis;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    [ExcludeFromCodeCoverage]
    public static class RandomVLanTaggedFrameExtensions
    {
        public static VLanTaggedFrameLayer NextVLanTaggedFrameLayer(this Random random, EthernetType etherType)
        {
            return new VLanTaggedFrameLayer
                   {
                       PriorityCodePoint = random.NextEnum<ClassOfService>(),
                       CanonicalFormatIndicator = random.NextBool(),
                       VLanIdentifier = random.NextUShort(VLanTaggedFrameDatagram.MaxVLanIdentifier + 1),
                       EtherType = etherType,
                   };
        }

        public static VLanTaggedFrameLayer NextVLanTaggedFrameLayer(this Random random)
        {
            return random.NextVLanTaggedFrameLayer(random.NextEnum(EthernetType.None));
        }
    }
}