namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public enum IcmpCodeDestinationUnrechable : byte
    {
        /// <summary>
        /// If, according to the information in the gateway's routing tables, 
        /// the network specified in the internet destination field of a datagram is unreachable, 
        /// e.g., the distance to the network is infinity, 
        /// the gateway may send a destination unreachable message to the internet source host of the datagram.
        /// </summary>
        NetUnreachable = 0x00,

        /// <summary>
        /// RFC 792.
        /// In some networks, the gateway may be able to determine if the internet destination host is unreachable.  
        /// Gateways in these networks may send destination unreachable messages to the source host when the destination host is unreachable.
        /// </summary>
        HostUnreachable = 0x01,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated protocol module is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        ProtocolUnreachable = 0x02,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated process port is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        PortUnreachable = 0x03,

        /// <summary>
        /// RFC 792.
        /// A datagram must be fragmented to be forwarded by a gateway yet the Don't Fragment flag is on.  
        /// In this case the gateway must discard the datagram and may return a destination unreachable message.
        /// </summary>
        FragmentationNeededAndDontFragmentSet = 0x04,

        /// <summary>
        /// RFC 792.
        /// </summary>
        SourceRouteFailed = 0x05,
    }
}
