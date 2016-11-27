using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// <pre>
    ///  Code   Len      NIS Domain Name
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  40 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpNetworkInformationServiceDomainOption : DhcpOption
    {
        public DhcpNetworkInformationServiceDomainOption(string nisDomainName) : base(DhcpOptionCode.NetworkInformationServiceDomain)
        {
            NISDomainName = nisDomainName;
        }

        internal static DhcpNetworkInformationServiceDomainOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            string hostName = Encoding.ASCII.GetString(data.ReadBytes(offset, len));
            DhcpNetworkInformationServiceDomainOption option = new DhcpNetworkInformationServiceDomainOption(hostName);
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(NISDomainName));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(NISDomainName);
            }
        }

        public string NISDomainName
        {
            get { return _nisDomainName; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(NISDomainName));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(NISDomainName), value.Length, "NISDomainName has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(NISDomainName), value.Length, "NISDomainName has to be less than 256 characters long");

                _nisDomainName = value;
            }
        }

        private string _nisDomainName;
    }
}