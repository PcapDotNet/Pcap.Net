using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option is used by a DHCP client to request values for specified
    /// configuration parameters.The list of requested parameters is
    /// specified as n octets, where each octet is a valid DHCP option code
    /// as defined in this document.
    /// The client MAY list the options in order of preference.The DHCP
    /// server is not required to return the options in the requested order,
    /// but MUST try to insert the requested options in the order requested
    /// by the client.
    /// <pre>
    /// Code   Len   Option Codes
    /// +-----+-----+-----+-----+---
    /// |  55 |  n  |  c1 |  c2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpParameterRequestListOption : DhcpOption
    {
        /// <summary>
        /// create new DhcpParameterRequestListOption
        /// </summary>
        /// <param name="optionCodes">Option Codes</param>
        public DhcpParameterRequestListOption(IList<DhcpOptionCode> optionCodes) : base(DhcpOptionCode.ParameterRequestList)
        {
            if (optionCodes == null)
                throw new ArgumentNullException(nameof(optionCodes));
            if (optionCodes.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(optionCodes), optionCodes.Count, "The minimum items in optionCodes is 1");
            if (optionCodes.Count > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(optionCodes), optionCodes.Count, "The maximum items in optionCodes is 255");
            OptionCodes = new ReadOnlyCollection<DhcpOptionCode>(optionCodes);
        }

        internal static DhcpParameterRequestListOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            List<DhcpOptionCode> codes = new List<DhcpOptionCode>();
            for (int i = 0; i < length; i++)
            {
                codes.Add((DhcpOptionCode)data[offset++]);
            }
            return new DhcpParameterRequestListOption(codes);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, OptionCodes.Select(p => (byte)p));
        }

        /// <summary>
        /// Length of the Dhcp-Option
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)OptionCodes.Count;
            }
        }

        /// <summary>
        /// RFC 2132.
        /// Option Codes
        /// </summary>
        public IReadOnlyCollection<DhcpOptionCode> OptionCodes
        {
            get;
            private set;
        }
    }
}