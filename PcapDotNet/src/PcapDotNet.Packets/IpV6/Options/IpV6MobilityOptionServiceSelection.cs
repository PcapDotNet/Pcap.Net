using System;
using System.Globalization;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5149.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Identifier                 |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.ServiceSelection)]
    public sealed class IpV6MobilityOptionServiceSelection : IpV6MobilityOptionSingleDataSegmentField
    {
        /// <summary>
        /// The minimum value that Identifier can be set to.
        /// </summary>
        public const int MinIdentifierLength = 1;

        /// <summary>
        /// The maximum value that Identifier can be set to.
        /// </summary>
        public const int MaxIdentifierLength = 255;

        /// <summary>
        /// Creates an instance from the given identifier.
        /// </summary>
        /// <param name="identifier">
        /// Encoded service identifier string used to identify the requested service.
        /// The identifier string length is between 1 and 255 octets.
        /// This specification allows international identifier strings that are based on the use of Unicode characters, encoded as UTF-8,
        /// and formatted using Normalization Form KC (NFKC).
        /// 
        /// 'ims', 'voip', and 'voip.companyxyz.example.com' are valid examples of Service Selection option Identifiers.
        /// At minimum, the Identifier must be unique among the home agents to which the mobile node is authorized to register.
        /// </param>
        public IpV6MobilityOptionServiceSelection(DataSegment identifier)
            : base(IpV6MobilityOptionType.ServiceSelection, identifier)
        {
            if (identifier.Length < MinIdentifierLength || identifier.Length > MaxIdentifierLength)
                throw new ArgumentOutOfRangeException("identifier", identifier,
                                                      string.Format(CultureInfo.InvariantCulture,
                                                                    "Identifier length must be at least {0} bytes long and at most {1} bytes long.",
                                                                    MinIdentifierLength, MaxIdentifierLength));
        }

        /// <summary>
        /// Encoded service identifier string used to identify the requested service.
        /// The identifier string length is between 1 and 255 octets.
        /// This specification allows international identifier strings that are based on the use of Unicode characters, encoded as UTF-8,
        /// and formatted using Normalization Form KC (NFKC).
        /// 
        /// 'ims', 'voip', and 'voip.companyxyz.example.com' are valid examples of Service Selection option Identifiers.
        /// At minimum, the Identifier must be unique among the home agents to which the mobile node is authorized to register.
        /// </summary>
        public DataSegment Identifier
        {
            get { return Value; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            if (data.Length < MinIdentifierLength || data.Length > MaxIdentifierLength)
                return null;

            return new IpV6MobilityOptionServiceSelection(data);
        }

        private IpV6MobilityOptionServiceSelection()
            : this(new DataSegment(new byte[1]))
        {
        }
    }
}