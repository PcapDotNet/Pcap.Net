using System.Collections.Generic;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class IpV4OptionRecordRoute : IpV4OptionRoute
    {
        public IpV4OptionRecordRoute(byte pointedAddressIndex, IList<IpV4Address> addresses)
            : base(IpV4OptionType.RecordRoute, addresses, pointedAddressIndex)
        {
        }

        public IpV4OptionRecordRoute(byte pointedAddressIndex, params IpV4Address[] addresses)
            : this(pointedAddressIndex, (IList<IpV4Address>)addresses)
        {
        }

        internal static IpV4OptionRecordRoute ReadOptionRecordRoute(byte[] buffer, ref int offset, byte valueLength)
        {
            IpV4Address[] addresses;
            byte pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, valueLength))
                return null;
            return new IpV4OptionRecordRoute(pointedAddressIndex, addresses);
        }
    }
}