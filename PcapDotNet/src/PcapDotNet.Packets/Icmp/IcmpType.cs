namespace PcapDotNet.Packets.Icmp
{
    public enum IcmpType : byte
    {
        /// <summary>
        /// RFC 792
        /// 
        /// <para>
        /// If, according to the information in the gateway's routing tables, 
        /// the network specified in the internet destination field of a datagram is unreachable, e.g., the distance to the network is infinity, 
        /// the gateway may send a destination unreachable message to the internet source host of the datagram.  
        /// In addition, in some networks, the gateway may be able to determine if the internet destination host is unreachable.  
        /// Gateways in these networks may send destination unreachable messages to the source host when the destination host is unreachable.
        /// </para>
        /// 
        /// <para>
        /// If, in the destination host, the IP module cannot deliver the datagram  because the indicated protocol module or process port is not active, 
        /// the destination host may send a destination unreachable message to the source host.
        /// </para>
        /// 
        /// <para>
        /// Another case is when a datagram must be fragmented to be forwarded by a gateway yet the Don't Fragment flag is on. 
        /// In this case the gateway must discard the datagram and may return a destination unreachable message.
        /// </para>
        /// 
        /// <para>
        /// Codes 0, 1, 4, and 5 may be received from a gateway.  
        /// Codes 2 and 3 may be received from a host.
        /// </para>
        /// </summary>
        DestinationUnreachable = 0x03
    }
}