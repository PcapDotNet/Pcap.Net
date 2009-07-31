namespace Packets
{
    public class IpV4OptionStrictSourceRouting : IpV4OptionRoute
    {
        public IpV4OptionStrictSourceRouting(IpV4Address[] addresses, byte pointedAddressIndex)
            : base(IpV4OptionType.StrictSourceRouting, addresses, pointedAddressIndex)
        {
        }

        internal static IpV4OptionStrictSourceRouting ReadOptionStrictSourceRouting(byte[] buffer, ref int offset, int length)
        {
            IpV4Address[] addresses;
            byte pointedAddressIndex;
            if (!TryRead(out addresses, out pointedAddressIndex, buffer, ref offset, length))
                return null;
            return new IpV4OptionStrictSourceRouting(addresses, pointedAddressIndex);
        }
    }
}