namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    ///  RFCs 2453, 2858, 4761, 6074.
    /// </summary>
    public enum AddressFamily : ushort
    {
        /// <summary>
        /// IP (IP version 4).
        /// </summary>
        IpV4 = 1,

        /// <summary>
        /// IP6 (IP version 6).
        /// </summary>
        IpV6 = 2,

        /// <summary>
        /// Network Service Access Point.
        /// </summary>
        Nsap = 3,

        /// <summary>
        /// High-Level Data Link (8-bit multidrop).
        /// </summary>
        Hdlc = 4,

        /// <summary>
        /// BBN Report 1822.
        /// </summary>
        Bbn1822 = 5,

        /// <summary>
        /// 802 (includes all 802 media plus Ethernet "canonical format").
        /// </summary>
        Media802 = 6,

        /// <summary>
        /// E.163.
        /// </summary>
        E163 = 7,

        /// <summary>
        /// E.164 (SMDS, Frame Relay, ATM).
        /// </summary>
        E164 = 8,

        /// <summary>
        /// F.69 (Telex).
        /// </summary>
        F69 = 9,

        /// <summary>
        /// X.121 (X.25, Frame Relay).
        /// </summary>
        X121 = 10,

        /// <summary>
        /// IPX.
        /// </summary>
        Ipx = 11,

        /// <summary>
        /// Appletalk.
        /// </summary>
        AppleTalk = 12,

        /// <summary>
        /// Decnet IV.
        /// </summary>
        DecnetIv = 13,	

        /// <summary>
        /// Banyan Vines.
        /// </summary>
        BanyanVines	= 14,

        /// <summary>
        /// E.164 with NSAP format subaddress.
        /// ATM Forum UNI 3.1. October 1995.
        /// Andy Malis.
        /// </summary>
        E164WithNsapFormatSubaddresses = 15,

        /// <summary>
        /// DNS (Domain Name System).
        /// </summary>
        Dns = 16,

        /// <summary>
        /// Distinguished Name.
        /// Charles Lynn.
        /// </summary>
        DistinguishedName = 17,

        /// <summary>
        /// AS Number.
        /// Charles Lynn.
        /// </summary>
        AsNumber = 18,

        /// <summary>
        /// XTP over IP version 4.
        /// Mike Saul.
        /// </summary>
        XtpOverIpV4 = 19,

        /// <summary>
        /// XTP over IP version 6.
        /// Mike Saul.
        /// </summary>
        XtpOverIpV6 = 20,

        /// <summary>
        /// XTP native mode XTP.
        /// Mike Saul.
        /// </summary>
        XtpNativeModeXtp = 21,

        /// <summary>
        /// Fibre Channel World-Wide Port Name.
        /// Mark Bakke.
        /// </summary>
        FibreChannelWorldWidePortName = 22,

        /// <summary>
        /// Fibre Channel World-Wide Node Name.
        /// Mark Bakke.
        /// </summary>
        FibreChannelWorldWideNodeName = 23,

        /// <summary>
        /// GWID.
        /// Subra Hegde.
        /// </summary>
        Gwis = 24,

        /// <summary>
        /// RFCs 4761, 6074.
        /// AFI for L2VPN information.
        /// </summary>
        AfiForL2VpnInformation = 25,

        /// <summary>
        /// EIGRP Common Service Family.
        /// Donnie Savage.
        /// </summary>
        EigrpCommonServiceFamily = 16384,

        /// <summary>
        /// EIGRP IPv4 Service Family.
        /// Donnie Savage.
        /// </summary>
        EigrpIpV4ServiceFamily = 16385,

        /// <summary>
        /// EIGRP IPv6 Service Family.
        /// Donnie Savage.
        /// </summary>
        EigrpIpV6ServiceFamily = 16386,

        /// <summary>
        /// LISP Canonical Address Format (LCAF).
        /// David Meyer.
        /// </summary>
        LispCanonicalAddressFormat = 16387,
    }
}