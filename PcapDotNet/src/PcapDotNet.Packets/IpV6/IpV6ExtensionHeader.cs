using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2460.
    /// </summary>
    public abstract class IpV6ExtensionHeader
    {
        protected IpV6ExtensionHeader(IpV4Protocol nextHeader)
        {
            NextHeader = nextHeader;
        }

        public IpV4Protocol NextHeader { get; private set; }
    }
}