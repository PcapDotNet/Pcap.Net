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
    ///  Code   Len   Vendor class Identifier
    /// +-----+-----+-----+-----+---
    /// |  60 |  n  |  i1 |  i2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpVendorClassidentifierOption : DhcpOption
    {
        public DhcpVendorClassidentifierOption(DataSegment vendorClassIdentifier) : base(DhcpOptionCode.VendorClassidentifier)
        {
            VendorClassIdentifier = vendorClassIdentifier;
        }

        internal static DhcpVendorClassidentifierOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            DhcpVendorClassidentifierOption option = new DhcpVendorClassidentifierOption(data.Subsegment(offset, length));
            offset += option.Length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, VendorClassIdentifier);
        }

        public override byte Length
        {
            get
            {
                if (VendorClassIdentifier == null)
                {
                    return 0;
                }
                return (byte)VendorClassIdentifier.Length;
            }
        }

        public DataSegment VendorClassIdentifier
        {
            get { return _vendorClassIdentifier; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(VendorClassIdentifier));
                }
                if (value.Length < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(VendorClassIdentifier), value.Length, "VendorClassIdentifier.Length has to be greater than 0");
                }
                if (value.Length > byte.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(VendorClassIdentifier), value.Length, "VendorClassIdentifier.Length has to be less than 256");
                }
                _vendorClassIdentifier = value;
            }
        }

        private DataSegment _vendorClassIdentifier;
    }
}