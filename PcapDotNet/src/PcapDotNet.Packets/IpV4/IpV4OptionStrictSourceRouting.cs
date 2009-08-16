using System.Collections.Generic;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Strict Source and Record Route
    /// +--------+--------+--------+---------//--------+
    /// |10001001| length | pointer|     route data    |
    /// +--------+--------+--------+---------//--------+
    ///  Type=137
    /// 
    /// The strict source and record route (SSRR) option provides a means for the source of an internet datagram 
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
    /// 
    /// If the address in destination address field has been reached and the pointer is not greater than the length, 
    /// the next address in the source route replaces the address in the destination address field, 
    /// and the recorded route address replaces the source address just used, and pointer is increased by four.
    /// 
    /// The recorded route address is the internet module's own internet address as known in the environment 
    /// into which this datagram is being forwarded.
    /// 
    /// This procedure of replacing the source route with the recorded route 
    /// (though it is in the reverse of the order it must be in to be used as a source route) 
    /// means the option (and the IP header as a whole) remains a constant length as the datagram progresses through the internet.
    /// 
    /// This option is a strict source route because the gateway or host IP 
    /// must send the datagram directly to the next address in the source route through only the directly connected network 
    /// indicated in the next address to reach the next gateway or host specified in the route.
    /// 
    /// Must be copied on fragmentation.  
    /// Appears at most once in a datagram.
    /// </summary>
    [OptionTypeRegistration(typeof(IpV4OptionType), IpV4OptionType.StrictSourceRouting)]
    public class IpV4OptionStrictSourceRouting : IpV4OptionRoute, IOptionComplexFactory
    {
        /// <summary>
        /// Create the option according to the given values.
        /// </summary>
        public IpV4OptionStrictSourceRouting(IList<IpV4Address> route, byte pointedAddressIndex)
            : base(IpV4OptionType.StrictSourceRouting, route, pointedAddressIndex)
        {
        }

        /// <summary>
        /// Creates an empty strict source routing option.
        /// </summary>
        public IpV4OptionStrictSourceRouting()
            : this(new List<IpV4Address>(), 0)
        {
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        public Option CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            IpV4Address[] addresses;
            byte pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, valueLength))
                return null;
            return new IpV4OptionStrictSourceRouting(addresses, pointedAddressIndex);
        }
    }
}