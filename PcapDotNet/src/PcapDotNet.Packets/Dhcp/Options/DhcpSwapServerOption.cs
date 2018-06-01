using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This specifies the IP address of the client's swap server.
    /// <pre>
    ///  Code   Len    Swap Server Address
    /// +-----+-----+-----+-----+-----+-----+
    /// |  16 |  4  |  a1 |  a2 |  a3 |  a4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpSwapServerOption : DhcpSingleAddressOption
    {
        /// <summary>
        /// create new DhcpSwapServerOption.
        /// </summary>
        /// <param name="swapServerAddress">Swap Server Address</param>
        public DhcpSwapServerOption(IpV4Address swapServerAddress) : base(swapServerAddress, DhcpOptionCode.SwapServer)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.SwapServer)]
        internal static DhcpSwapServerOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpSwapServerOption>(data, ref offset, p => new DhcpSwapServerOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Swap Server Address.
        /// </summary>
        public IpV4Address SwapServerAddress
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}