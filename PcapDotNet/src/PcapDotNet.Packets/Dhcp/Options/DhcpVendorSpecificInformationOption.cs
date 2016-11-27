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
    /// Code   Len   Vendor-specific information
    /// +-----+-----+-----+-----+---
    /// |  43 |  n  |  i1 |  i2 | ...
    /// +-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpVendorSpecificInformationOption : DhcpOption
    {
        public DhcpVendorSpecificInformationOption(DataSegment vendorSpecificInformation) : base(DhcpOptionCode.VendorSpecificInformation)
        {
            VendorSpecificInformation = vendorSpecificInformation;
        }

        internal static DhcpVendorSpecificInformationOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            DhcpVendorSpecificInformationOption option = new DhcpVendorSpecificInformationOption(data.Subsegment(offset, length));
            offset += length;
            return option;
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            if (VendorSpecificInformation == null)
                throw new ArgumentNullException(nameof(VendorSpecificInformation));
            if (VendorSpecificInformation.Length > byte.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(VendorSpecificInformation), VendorSpecificInformation.Length, "VendorSpecificInformation.Length has to be less than 256");
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, VendorSpecificInformation);
        }

        public override byte Length
        {
            get
            {
                if (VendorSpecificInformation == null)
                {
                    return 0;
                }
                return (byte)VendorSpecificInformation.Length;
            }
        }

        public DataSegment VendorSpecificInformation
        {
            get { return _vendorSpecificInformation; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(VendorSpecificInformation));
                if (value.Length > byte.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(VendorSpecificInformation), value.Length, "VendorSpecificInformation.Length has to be less than 256");
                _vendorSpecificInformation = value;
            }
        }

        private DataSegment _vendorSpecificInformation;
    }
}