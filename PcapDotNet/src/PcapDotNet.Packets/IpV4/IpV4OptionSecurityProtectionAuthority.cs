using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// This field identifies the National Access Programs or Special Access Programs 
    /// which specify protection rules for transmission and processing of the information contained in the datagram. 
    /// Protection authority flags do NOT represent accreditation authorities, though the semantics are superficially similar.  
    /// </summary>
    [Flags]
    public enum IpV4OptionSecurityProtectionAuthority : byte
    {
        /// <summary>
        /// No protection authorities.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Designated Approving Authority per DOD 5200.28
        /// </summary>
        Genser = 0x80,

        /// <summary>
        /// Single Integrated Operational Plan - Extremely Sensitive Information (SIOP-ESI).
        /// Department of Defense Organization of the Joint Chiefs of Staff 
        /// Attn: J6 Washington, DC  20318-6000
        /// </summary>
        SingleIntegrationOptionalPlanExtremelySensitiveInformation = 0x40,

        /// <summary>
        /// Sensitive Compartmented Information (SCI).
        /// Director of Central Intelligence 
        /// Attn: Chairman, Information Handling Committee, Intelligence Community Staff Washington, D.C. 20505
        /// </summary>
        SensitiveCompartmentedInformation = 0x20,

        /// <summary>
        /// National Security Agency (NSA).
        /// 9800 Savage Road Attn: T03 Ft. Meade, MD 20755-6000
        /// </summary>
        Nsa = 0x10,

        /// <summary>
        /// Department of Energy (DOE).
        /// Attn:  DP343.2 Washington, DC  20545
        /// </summary>
        DeparmentOfEnergy = 0x08
    }
}