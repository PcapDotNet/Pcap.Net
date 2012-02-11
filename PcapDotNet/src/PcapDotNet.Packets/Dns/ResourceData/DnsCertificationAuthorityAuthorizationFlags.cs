using System;

namespace PcapDotNet.Packets.Dns
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags"), Flags]
    public enum DnsCertificationAuthorityAuthorizationFlags : byte
    {
        /// <summary>
        /// If set, the critical flag is asserted and the property must be understood if the CAA record is to be correctly processed by a certificate issuer.
        /// A Certification Authority must not issue certificates for any Domain that contains a CAA critical property for an unknown or unsupported property type that has the issuer flag set.
        /// </summary>
        Critical = 0x80,
    }
}