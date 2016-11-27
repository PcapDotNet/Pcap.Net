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
    ///  Code   Len      NIS Client Domain Name
    /// +-----+-----+-----+-----+-----+-----+---
    /// |  64 |  n  |  n1 |  n2 |  n3 |  n4 | ...
    /// +-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpNetworkInformationServicePlusDomainOption : DhcpOption
    {
        public DhcpNetworkInformationServicePlusDomainOption(string domainName) : base(DhcpOptionCode.NetworkInformationServicePlusDomain)
        {
            DomainName = domainName;
        }

        internal static DhcpNetworkInformationServicePlusDomainOption Read(DataSegment data, ref int offset)
        {
            byte len = data[offset++];
            DhcpNetworkInformationServicePlusDomainOption option = new DhcpNetworkInformationServicePlusDomainOption(Encoding.ASCII.GetString(data.ReadBytes(offset, len)));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(DomainName));
        }

        public override byte Length
        {
            get
            {
                return (byte)Encoding.ASCII.GetByteCount(DomainName);
            }
        }

        public string DomainName
        {
            get { return _domainName; }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(DomainName));
                if (value.Length < 1)
                    throw new ArgumentOutOfRangeException(nameof(DomainName), value.Length, "DomainName has to be at least 1 characters long");
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(DomainName), value.Length, "DomainName has to be less than 256 characters long");

                _domainName = value;
            }
        }

        private string _domainName;
    }
}