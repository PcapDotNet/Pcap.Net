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
    public class IpV6OptionIlnpNonce : IpV6OptionComplex
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

        internal IpV6OptionIlnpNonce()
            : this(DataSegment.Empty)
        {
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            return new IpV6OptionIlnpNonce(data);
        }

        internal override int DataLength
        {
            get { return Nonce.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Nonce);
        }
    }
}