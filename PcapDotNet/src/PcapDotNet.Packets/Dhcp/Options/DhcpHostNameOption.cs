using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the name of the client. The name may or may
    /// not be qualified with the local domain name(see section 3.17 for the
    /// preferred way to retrieve the domain name). See RFC 1035 for
    /// character set restrictions.
    /// <pre>
    ///  Code   Len                 Host Name
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// |  12 |  n  |  h1 |  h2 |  h3 |  h4 |  h5 |  h6 |  ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+--
    /// </pre>
    /// </summary>
    public class DhcpHostNameOption : DhcpStringOption
    {
        /// <summary>
        /// create new DhcpHostNameOption.
        /// </summary>
        /// <param name="hostName">Host Name</param>
        public DhcpHostNameOption(string hostName) : base(hostName, DhcpOptionCode.HostName)
        {
        }

        [DhcpOptionReadRegistration(DhcpOptionCode.HostName)]
        internal static DhcpHostNameOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpHostNameOption>(data, ref offset, p => new DhcpHostNameOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// Host Name.
        /// </summary>
        public string HostName
        {
            get { return InternalValue; }
            set { InternalValue = value; }
        }
    }
}