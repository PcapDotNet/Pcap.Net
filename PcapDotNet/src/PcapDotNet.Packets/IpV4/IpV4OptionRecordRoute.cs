using System.Collections.Generic;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Record Route
    /// +--------+--------+--------+---------//--------+
    /// |00000111| length | pointer|     route data    |
    /// +--------+--------+--------+---------//--------+
    ///  Type=7
    /// 
    /// The record route option provides a means to record the route of an internet datagram.
    /// 
    /// The option begins with the option type code.  
    /// The second octet is the option length which includes the option type code and the length octet, 
    /// the pointer octet, and length-3 octets of route data.  
    /// The third octet is the pointer into the route data indicating the octet which begins the next area to store a route address.  
    /// The pointer is relative to this option, and the smallest legal value for the pointer is 4.
    /// 
    /// A recorded route is composed of a series of internet addresses.
    /// Each internet address is 32 bits or 4 octets.  
    /// If the pointer is greater than the length, the recorded route data area is full.
    /// The originating host must compose this option with a large enough route data area to hold all the address expected.  
    /// The size of the option does not change due to adding addresses.  
    /// The intitial contents of the route data area must be zero.
    /// 
    /// When an internet module routes a datagram it checks to see if the record route option is present.  
    /// If it is, it inserts its own internet address as known in the environment into which this datagram is being forwarded 
    /// into the recorded route begining at the octet indicated by the pointer, 
    /// and increments the pointer by four.
    /// 
    /// If the route data area is already full (the pointer exceeds the length) 
    /// the datagram is forwarded without inserting the address into the recorded route.  
    /// If there is some room but not enough room for a full address to be inserted, 
    /// the original datagram is considered to be in error and is discarded.  
    /// In either case an ICMP parameter problem message may be sent to the source host.
    /// 
    /// Not copied on fragmentation, goes in first fragment only.
    /// Appears at most once in a datagram.
    /// </summary>
    [OptionTypeRegistration(typeof(IpV4OptionType), IpV4OptionType.RecordRoute)]
    public class IpV4OptionRecordRoute : IpV4OptionRoute, IOptionComplexFactory
    {
        /// <summary>
        /// Constructs the option from the given values.
        /// </summary>
        /// <param name="route">The route addresses values.</param>
        /// <param name="pointedAddressIndex">The pointed index in the route.</param>
        public IpV4OptionRecordRoute(byte pointedAddressIndex, IList<IpV4Address> route)
            : base(IpV4OptionType.RecordRoute, route, pointedAddressIndex)
        {
        }

        /// <summary>
        /// Constructs the option from the given values.
        /// </summary>
        /// <param name="route">The route addresses values.</param>
        /// <param name="pointedAddressIndex">The pointed index in the route.</param>
        public IpV4OptionRecordRoute(byte pointedAddressIndex, params IpV4Address[] route)
            : this(pointedAddressIndex, (IList<IpV4Address>)route)
        {
        }

        /// <summary>
        /// Constructs an empty record route option.
        /// </summary>
        public IpV4OptionRecordRoute()
            : this(0)
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
            return new IpV4OptionRecordRoute(pointedAddressIndex, addresses);
        }
    }
}