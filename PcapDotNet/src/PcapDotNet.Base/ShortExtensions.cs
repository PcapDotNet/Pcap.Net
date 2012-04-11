using System.Net;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension method for Short structure.
    /// </summary>
    public static class ShortExtensions
    {
        public static short ReverseEndianity(this short value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }
    }
}