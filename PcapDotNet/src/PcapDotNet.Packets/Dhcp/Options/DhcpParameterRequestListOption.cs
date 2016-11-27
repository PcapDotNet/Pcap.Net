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
    /// <pre>
    /// Code   Len   Option Codes
    /// +-----+-----+-----+-----+---
    /// |  55 |  n  |  c1 |  c2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpParameterRequestListOption : DhcpOption
    {
        public DhcpParameterRequestListOption(IList<DhcpOptionCode> codes) : base(DhcpOptionCode.ParameterRequestList)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));
            if (codes.Count > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(codes), codes.Count, "The maximum items in codes is 255");
            Codes = new ReadOnlyCollection<DhcpOptionCode>(codes);
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
            buffer.Write(ref offset, Codes.Select(p => (byte)p));
        }

        public override byte Length
        {
            get
            {
                if (Codes.Count > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(Codes), Codes.Count, "The maximum items in addresses is 255");

                return (byte)Codes.Count;
            }
        }

        public IReadOnlyCollection<DhcpOptionCode> Codes
        {
            get;
            private set;
        }
    }
}