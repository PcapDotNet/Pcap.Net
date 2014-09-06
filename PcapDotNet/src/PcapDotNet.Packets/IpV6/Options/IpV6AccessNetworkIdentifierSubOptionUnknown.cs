using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+----------+------------+
    /// | Bit | 0-7      | 8-15       |
    /// +-----+----------+------------+
    /// | 0   | ANI Type | ANI Length |
    /// +-----+----------+------------+
    /// | 16  | Option Data           |
    /// | ... |                       |
    /// +-----+-----------------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6AccessNetworkIdentifierSubOptionUnknown : IpV6AccessNetworkIdentifierSubOption
    {
        public IpV6AccessNetworkIdentifierSubOptionUnknown(IpV6AccessNetworkIdentifierSubOptionType type, DataSegment data)
            : base(type)
        {
            Data = data;
        }

        public DataSegment Data { get; private set; }

        internal override IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data)
        {
            throw new InvalidOperationException("IpV6AccessNetworkIdentifierSubOptionUnknown shouldn't be registered.");
        }

        internal override int DataLength
        {
            get { return Data.Length; }
        }

        internal override bool EqualsData(IpV6AccessNetworkIdentifierSubOption other)
        {
            return EqualsData(other as IpV6AccessNetworkIdentifierSubOptionUnknown);
        }

        internal override int GetDataHashCode()
        {
            return Data.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Data);
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionUnknown other)
        {
            return other != null &&
                   Data.Equals(other.Data);
        }
    }
}