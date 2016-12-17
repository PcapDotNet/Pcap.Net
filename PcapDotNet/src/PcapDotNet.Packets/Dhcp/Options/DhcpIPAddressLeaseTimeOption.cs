using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used in a client request (DHCPDISCOVER or DHCPREQUEST)
    /// to allow the client to request a lease time for the IP address. In a
    /// server reply(DHCPOFFER), a DHCP server uses this option to specify
    /// the lease time it is willing to offer.
    /// <pre>
    ///  Code   Len         Lease Time
    /// +-----+-----+-----+-----+-----+-----+
    /// |  51 |  4  |  t1 |  t2 |  t3 |  t4 |
    /// +-----+-----+-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpIPAddressLeaseTimeOption : DhcpUIntOption
    {
        /// <summary>
        /// create new DhcpIPAddressLeaseTimeOption
        /// </summary>
        /// <param name="leaseTime">Lease Time</param>
        public DhcpIPAddressLeaseTimeOption(uint leaseTime) : base(leaseTime, DhcpOptionCode.IPAddressLeaseTime)
        {
        }

        internal static DhcpIPAddressLeaseTimeOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpIPAddressLeaseTimeOption>(data, ref offset, p => new Options.DhcpIPAddressLeaseTimeOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Lease Time
        /// The time is in units of seconds
        /// </summary>
        public uint LeaseTime
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}