namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC irtf-rrg-ilnp-noncev6-06.
    /// http://tools.ietf.org/html/draft-irtf-rrg-ilnp-noncev6-06
    /// IPv6 Nonce Destination Option for ILNPv6.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Nonce Value                |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.IlnpNonce)]
    public sealed class IpV6OptionIlnpNonce : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        public IpV6OptionIlnpNonce(DataSegment nonce)
            : base(IpV6OptionType.IlnpNonce)
        {
            Nonce = nonce;
        }

        /// <summary>
        /// An unpredictable cryptographically random value used to prevent off-path attacks on an ILNP session.
        /// </summary>
        public DataSegment Nonce { get; private set; }

        public IpV6Option CreateInstance(DataSegment data)
        {
            return new IpV6OptionIlnpNonce(data);
        }

        internal override int DataLength
        {
            get { return Nonce.Length; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionIlnpNonce);
        }

        internal override int GetDataHashCode()
        {
            return Nonce.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Nonce);
        }

        private IpV6OptionIlnpNonce()
            : this(DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6OptionIlnpNonce other)
        {
            return other != null &&
                   Nonce.Equals(other.Nonce);
        }
    }
}