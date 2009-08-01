using System.Collections.Generic;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class IpV4OptionStrictSourceRouting : IpV4OptionRoute
    {
        public IpV4OptionStrictSourceRouting(IList<IpV4Address> addresses, byte pointedAddressIndex)
            : base(IpV4OptionType.StrictSourceRouting, addresses, pointedAddressIndex)
        {
        }

        internal static IpV4OptionStrictSourceRouting ReadOptionStrictSourceRouting(byte[] buffer, ref int offset, byte valueLength)
        {
            IpV4Address[] addresses;
            byte pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, valueLength))
                return null;
            return new IpV4OptionStrictSourceRouting(addresses, pointedAddressIndex);
        }
    }
}