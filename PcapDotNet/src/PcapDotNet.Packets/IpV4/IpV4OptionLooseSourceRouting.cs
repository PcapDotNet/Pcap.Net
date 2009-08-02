using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Loose Source and Record Route
    /// +--------+--------+--------+---------//--------+
    /// |10000011| length | pointer|     route data    |
    /// +--------+--------+--------+---------//--------+
    ///  Type=131
    /// 
    /// The loose source and record route (LSRR) option provides a means for the source of an internet datagram 
    /// to supply routing information to be used by the gateways in forwarding the datagram to the destination, 
    /// and to record the route information.
    /// 
    /// The option begins with the option type code.  
    /// The second octet is the option length which includes the option type code and the length octet, 
    /// the pointer octet, and length-3 octets of route data.  
    /// The third octet is the pointer into the route data indicating the octet which begins the next source address to be processed.  
    /// The pointer is relative to this option, and the smallest legal value for the pointer is 4.
    /// 
    /// A route data is composed of a series of internet addresses.
    /// Each internet address is 32 bits or 4 octets.  
    /// If the pointer is greater than the length, the source route is empty (and the recorded route full) 
    /// and the routing is to be based on the destination address field.
    /// If the address in destination address field has been reached and the pointer is not greater than the length, 
    /// the next address in the source route replaces the address in the destination address field, 
    /// and the recorded route address replaces the source address just used, and pointer is increased by four.
    /// 
    /// The recorded route address is the internet module's own internet address 
    /// as known in the environment into which this datagram is being forwarded.
    /// 
    /// This procedure of replacing the source route with the recorded route 
    /// (though it is in the reverse of the order it must be in to be used as a source route) means the option (and the IP header as a whole) 
    /// remains a constant length as the datagram progresses through the internet.
    /// 
    /// This option is a loose source route because the gateway or host IP 
    /// is allowed to use any route of any number of other intermediate gateways to reach the next address in the route.
    /// 
    /// Must be copied on fragmentation.  
    /// Appears at most once in a datagram.
    /// </summary>
    public class IpV4OptionLooseSourceRouting : IpV4OptionRoute
    {
        public IpV4OptionLooseSourceRouting(IList<IpV4Address> addresses, byte pointedAddressIndex)
            : base(IpV4OptionType.LooseSourceRouting, addresses, pointedAddressIndex)
        {
        }

        internal static IpV4OptionLooseSourceRouting ReadOptionLooseSourceRouting(byte[] buffer, ref int offset, byte valueLength)
        {
            IpV4Address[] addresses;
            byte pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, valueLength))
                return null;
            return new IpV4OptionLooseSourceRouting(addresses, pointedAddressIndex);
        }
    }
}