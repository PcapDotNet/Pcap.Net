namespace PcapDotNet.Packets.Icmp
{
    public enum IcmpCodeTimeExceeded : byte
    {
        /// <summary>
        /// RFC 792.
        /// If the gateway processing a datagram finds the time to live field is zero it must discard the datagram.  
        /// The gateway may also notify the source host via the time exceeded message.
        /// </summary>
        TimeToLive = 0x00,

        /// <summary>
        /// RFC 792.
        /// If a host reassembling a fragmented datagram cannot complete the reassembly due to missing fragments within its time limit it discards the datagram, 
        /// and it may send a time exceeded message.
        /// If fragment zero is not available then no time exceeded need be sent at all.
        /// </summary>
        FragmentReassembly = 0x01,
    }
}