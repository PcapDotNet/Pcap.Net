using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | ANI Sub-option(s)          |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.AccessNetworkIdentifier)]
    public sealed class IpV6MobilityOptionAccessNetworkIdentifier : IpV6MobilityOptionComplex
    {
        /// <summary>
        /// Creates an instance from sub options.
        /// </summary>
        /// <param name="subOptions">Sub options.</param>
        public IpV6MobilityOptionAccessNetworkIdentifier(IpV6AccessNetworkIdentifierSubOptions subOptions)
            : base(IpV6MobilityOptionType.AccessNetworkIdentifier)
        {
            if (subOptions.BytesLength > MaxDataLength)
                throw new ArgumentOutOfRangeException("subOptions", subOptions, string.Format("SubOptions take more than {0} bytes", MaxDataLength));
            SubOptions = subOptions;
        }

        /// <summary>
        /// Sub options.
        /// </summary>
        public IpV6AccessNetworkIdentifierSubOptions SubOptions { get; private set; }

        /// <summary>
        /// True iff parsing of this option didn't encounter issues.
        /// </summary>
        public override bool IsValid
        {
            get { return SubOptions.IsValid; }
        }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            return new IpV6MobilityOptionAccessNetworkIdentifier(new IpV6AccessNetworkIdentifierSubOptions(data));
        }

        internal override int DataLength
        {
            get { return SubOptions.BytesLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionAccessNetworkIdentifier);
        }

        internal override int GetDataHashCode()
        {
            return SubOptions.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            SubOptions.Write(buffer, offset);
            offset += DataLength;
        }

        private IpV6MobilityOptionAccessNetworkIdentifier()
            : this(IpV6AccessNetworkIdentifierSubOptions.None)
        {
        }

        private bool EqualsData(IpV6MobilityOptionAccessNetworkIdentifier other)
        {
            return other != null &&
                   SubOptions.Equals(other.SubOptions);
        }
    }
}