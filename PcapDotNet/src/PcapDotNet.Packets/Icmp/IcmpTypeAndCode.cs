namespace PcapDotNet.Packets.Icmp
{
    public enum IcmpTypeAndCode : ushort
    {
        /// <summary>
        /// RFC 792.
        /// If, according to the information in the gateway's routing tables, 
        /// the network specified in the internet destination field of a datagram is unreachable, 
        /// e.g., the distance to the network is infinity, 
        /// the gateway may send a destination unreachable message to the internet source host of the datagram.
        /// </summary>
        DestinationUnreachableNetUnreachable = 0x0300,

        /// <summary>
        /// RFC 792.
        /// In some networks, the gateway may be able to determine if the internet destination host is unreachable.  
        /// Gateways in these networks may send destination unreachable messages to the source host when the destination host is unreachable.
        /// </summary>
        DestinationUnreachableHostUnreachable = 0x0301,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated protocol module is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        DestinationUnreachableProtocolUnreachable = 0x0302,

        /// <summary>
        /// RFC 792.
        /// If, in the destination host, the IP module cannot deliver the datagram because the indicated process port is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </summary>
        DestinationUnreachablePortUnreachable = 0x0303,

        /// <summary>
        /// RFC 792.
        /// A datagram must be fragmented to be forwarded by a gateway yet the Don't Fragment flag is on.  
        /// In this case the gateway must discard the datagram and may return a destination unreachable message.
        /// </summary>
        DestinationUnreachableFragmentationNeededAndDontFragmentSet = 0x0304,

        DestinationUnreachableSourceRouteFailed = 0x0305,
    }
}