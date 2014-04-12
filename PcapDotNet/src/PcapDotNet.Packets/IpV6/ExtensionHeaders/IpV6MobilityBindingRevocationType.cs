namespace PcapDotNet.Packets.IpV6
{
    public enum IpV6MobilityBindingRevocationType : byte
    {
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