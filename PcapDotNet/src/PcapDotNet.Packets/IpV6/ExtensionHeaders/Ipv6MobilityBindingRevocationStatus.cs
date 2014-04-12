namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Values of the Status field less than 128 indicate that the Binding Revocation Indication was processed successfully by the responder.
    /// Values greater than or equal to 128 indicate that the Binding Revocation Indication was rejected by the responder. 
    /// </summary>
    public enum Ipv6MobilityBindingRevocationStatus : byte
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Partial success.
        /// </summary>
        PartialSuccess = 1,

        /// <summary>
        /// Binding Does NOT Exist.
        /// </summary>
        BindingDoesNotExist = 128,

        /// <summary>
        /// IPv4 Home Address Option Required.
        /// </summary>
        IpV4HomeAddressOptionRequired = 129,

        /// <summary>
        /// Global Revocation NOT Authorized.
        /// </summary>
        GlobalRevocationNotAuthorized = 130,

        /// <summary>
        /// Revoked Mobile Nodes Identity Required.
        /// </summary>
        RevokedMobileNodesIdentityRequired = 131,

        /// <summary>
        /// Revocation Failed - MN is Attached.
        /// </summary>
        RevocationFailedMobilityNodeIsAttached = 132,

        /// <summary>
        /// Revocation Trigger NOT Supported.
        /// </summary>
        RevocationTriggerNotSupported = 133,

        /// <summary>
        /// Revocation Function NOT Supported.
        /// </summary>
        RevocationFunctionNotSupported = 134,

        /// <summary>
        /// Proxy Binding Revocation NOT Supported.
        /// </summary>
        ProxyBindingRevocationNotSupported = 135,
    }
}