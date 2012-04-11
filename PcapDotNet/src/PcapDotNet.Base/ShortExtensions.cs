using System.Net;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension method for Short structure.
    /// </summary>
    public static class ShortExtensions
    {
        /// <summary>
        /// Reverses the endianity of the given value.
        /// </summary>
        public static short ReverseEndianity(this short value)
        {
            return IPAddress.HostToNetworkOrder(value);
        }
    }
}