namespace PcapDotNet.Packets
{
    public class IpV4OptionRecordRoute : IpV4OptionRoute
    {
        public IpV4OptionRecordRoute(IpV4Address[] addresses, byte pointedAddressIndex)
            : base(IpV4OptionType.RecordRoute, addresses, pointedAddressIndex)
        {
        }

        internal static IpV4OptionRecordRoute ReadOptionRecordRoute(byte[] buffer, ref int offset, int length)
        {
            IpV4Address[] addresses;
            byte pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, length))
                return null;
            return new IpV4OptionRecordRoute(addresses, pointedAddressIndex);
        }
    }
}