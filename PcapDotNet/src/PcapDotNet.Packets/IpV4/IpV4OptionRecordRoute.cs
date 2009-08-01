using System.Collections.Generic;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class IpV4OptionRecordRoute : IpV4OptionRoute
    {
        public IpV4OptionRecordRoute(IList<IpV4Address> addresses, byte pointedAddressIndex)
            : base(IpV4OptionType.RecordRoute, addresses, pointedAddressIndex)
        {
        }

        internal static IpV4OptionRecordRoute ReadOptionRecordRoute(byte[] buffer, ref int offset, byte valueLength)
        {
            IpV4Address[] addresses;
            byte pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, valueLength))
                return null;
            return new IpV4OptionRecordRoute(addresses, pointedAddressIndex);
        }
    }
}