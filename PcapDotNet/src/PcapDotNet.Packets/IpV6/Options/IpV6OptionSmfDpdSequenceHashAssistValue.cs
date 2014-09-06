namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6621.
    /// Simplified Multicast Forwarding Duplicate Packet Detection.
    /// Hash assist value.
    /// <pre>
    /// +-----+---+----------+
    /// | Bit | 0 | 1-7      |
    /// +-----+---+----------+
    /// | 0   | Option Type  |
    /// +-----+--------------+
    /// | 8   | Opt Data Len |
    /// +-----+---+----------+
    /// | 16  | 1 | Hash     |
    /// +-----+---+ Assist   |
    /// | ... | Value (HAV)  |
    /// +-----+--------------+
    /// </pre>
    /// </summary>
    public sealed class IpV6OptionSmfDpdSequenceHashAssistValue : IpV6OptionSmfDpd
    {
        private static class Offset
        {
            public const int HashAssistValue = 0;
        }

        public IpV6OptionSmfDpdSequenceHashAssistValue(DataSegment data)
        {
            byte[] hashAssistValueBuffer = new byte[data.Length - Offset.HashAssistValue];
            data.Buffer.BlockCopy(data.StartOffset + Offset.HashAssistValue, hashAssistValueBuffer, 0, hashAssistValueBuffer.Length);
            hashAssistValueBuffer[0] &= 0x7F;
            HashAssistValue = new DataSegment(hashAssistValueBuffer);
        }

        /// <summary>
        /// Hash assist value (HAV) used to facilitate H-DPD operation.
        /// </summary>
        public DataSegment HashAssistValue { get; private set; }

        public override bool HashIndicator
        {
            get { return true; }
        }

        internal override int DataLength
        {
            get { return HashAssistValue.Length; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionSmfDpdSequenceHashAssistValue);
        }

        internal override int GetDataHashCode()
        {
            return HashAssistValue.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (byte)(HashAssistValue[0] | 0x80));
            buffer.Write(ref offset, HashAssistValue.Subsegment(1, HashAssistValue.Length - 1));
        }

        private bool EqualsData(IpV6OptionSmfDpdSequenceHashAssistValue other)
        {
            return other != null &&
                   HashAssistValue.Equals(other.HashAssistValue);
        }
    }
}