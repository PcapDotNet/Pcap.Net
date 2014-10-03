using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Core.Test
{
    internal static class IpV6AddressExtensions
    {
        public static string GetWiresharkString(this IpV6Address address)
        {
            string str = address.ToString("x");
            if (str.StartsWith("0:0:"))
                str = "::" + str.Substring(4);
            if (str.EndsWith(":0:0"))
                str = str.Substring(0, str.Length - 4) + "::";
            str = str.Replace(":0:0:", "::");
            return str;
        }
    }
}