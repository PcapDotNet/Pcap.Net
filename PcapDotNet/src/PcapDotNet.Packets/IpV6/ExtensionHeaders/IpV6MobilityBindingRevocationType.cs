namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Binding Revocation Type of a Binding Revocation Message IPv6 Extension Header.
    /// The specific type of the extension header is decided according to this type.
    /// </summary>
    public enum IpV6MobilityBindingRevocationType : byte
    {
        /// <summary>
        /// Invalid value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Binding Revocation Indication.
        /// </summary>
        BindingRevocationIndication = 1,

        /// <summary>
        /// Binding Revocation Acknowledgement.
        /// </summary>
        BindingRevocationAcknowledgement = 2,
    }
}