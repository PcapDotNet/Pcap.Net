using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// </summary>
    public enum DhcpOptionCode : byte
    {
        #region 3. RFC 1497 Vendor Extensions

        /// <summary>
        /// RFC 2132.
        /// <pre>
        /// +-----+
        /// |  0  |
        /// +-----+
        /// </pre>
        /// </summary>
        Pad = 0,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        /// +-----+
        /// | 255 |
        /// +-----+
        /// </pre>
        /// </summary>
        End = 255,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len        Subnet Mask
        /// +-----+-----+-----+-----+-----+-----+
        /// |  1  |  4  |  m1 |  m2 |  m3 |  m4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        SubnetMask = 1,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len        Time Offset
        /// +-----+-----+-----+-----+-----+-----+
        /// |  1  |  4  |  n1 |  n2 |  n3 |  n4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        TimeOffset = 2,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  3  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        Router = 3,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  4  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        TimeServer = 4,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  5  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        NameServer = 5,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  6  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        DomainNameServerServer = 6,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  7  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        LogServer = 7,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  8  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        CookieServer = 8,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  9  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        LPRServer = 9,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  10 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        ImpressServer = 10,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  11 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        ResourceLocationServer = 11,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len                 Host Name
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  12 |  n  |  h1 |  h2 |  h3 |  h4 |  h5 |  h6 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        HostName = 12,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len   File Size
        /// +-----+-----+-----+-----+
        /// |  13 |  2  |  l1 |  l2 |
        /// +-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        BootFileSize = 13,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len      Dump File Pathname
        /// +-----+-----+-----+-----+-----+-----+---
        /// |  14 |  n  |  n1 |  n2 |  n3 |  n4 | ...
        /// +-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        MeritDumpFile = 14,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len        Domain Name
        /// +-----+-----+-----+-----+-----+-----+---
        /// |  15 |  n  |  d1 |  d2 |  d3 |  d4 | ...
        /// +-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        DomainName = 15,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len    Swap Server Address
        /// +-----+-----+-----+-----+-----+-----+
        /// |  16 |  4  |  a1 |  a2 |  a3 |  a4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        SwapServer = 16,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len      Root Disk Pathname
        /// +-----+-----+-----+-----+-----+-----+---
        /// |  17 |  n  |  n1 |  n2 |  n3 |  n4 | ...
        /// +-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        RootPath = 17,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len      Extensions Pathname
        /// +-----+-----+-----+-----+-----+-----+---
        /// |  18 |  n  |  n1 |  n2 |  n3 |  n4 | ...
        /// +-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        ExtensionsPath = 18,

        #endregion 3. RFC 1497 Vendor Extensions

        #region 4. IP Layer Parameters per Host

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  19 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        IPForwardingEnable = 19,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  20 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        NonLocalSourceRoutingEnable = 20,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1                  Mask 1
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
        /// |  21 |  n  |  a1 |  a2 |  a3 |  a4 |  m1 |  m2 |  m3 |  m4 |
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
        ///         Address 2                  Mask 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// |  a1 |  a2 |  a3 |  a4 |  m1 |  m2 |  m3 |  m4 | ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        PolicyFilter = 21,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len      Size
        /// +-----+-----+-----+-----+
        /// |  22 |  2  |  s1 |  s2 |
        /// +-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        MaximumDatagramReassemblySize = 22,

        /// <summary>
        /// RFC 2132
        /// <pre>
        ///  Code   Len   TTL
        /// +-----+-----+-----+
        /// |  23 |  1  | ttl |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        DefaultIPTimeToLive = 23,

        /// <summary>
        /// RFC 2132
        /// <pre>
        ///  Code   Len           Timeout
        /// +-----+-----+-----+-----+-----+-----+
        /// |  24 |  4  |  t1 |  t2 |  t3 |  t4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        PathMTUAgingTimeout = 24,

        /// <summary>
        /// RFC 2132
        /// <pre>
        ///  Code   Len     Size 1      Size 2
        /// +-----+-----+-----+-----+-----+-----+---
        /// |  25 |  n  |  s1 |  s2 |  s1 |  s2 | ...
        /// +-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        PathMTUPlateauTable = 25,

        #endregion 4. IP Layer Parameters per Host

        #region 5. IP Layer Parameters per Interface

        /// <summary>
        /// RFC 2132
        /// <pre>
        ///  Code   Len      MTU
        /// +-----+-----+-----+-----+
        /// |  26 |  2  |  m1 |  m2 |
        /// +-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        InterfaceMTU = 26,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  27 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        AllSubnetsAreLocal = 27,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len     Broadcast Address
        /// +-----+-----+-----+-----+-----+-----+
        /// |  28 |  4  |  b1 |  b2 |  b3 |  b4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        BroadcastAddress = 28,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  29 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        PerformMaskDiscovery = 29,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  30 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        MaskSupplier = 30,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  31 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        PerformRouterDiscovery = 31,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len            Address
        /// +-----+-----+-----+-----+-----+-----+
        /// |  32 |  4  |  a1 |  a2 |  a3 |  a4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        RouterSolicitationAddress = 32,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Destination 1           Router 1
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
        /// |  33 |  n  |  d1 |  d2 |  d3 |  d4 |  r1 |  r2 |  r3 |  r4 |
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
        ///         Destination 2           Router 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// |  d1 |  d2 |  d3 |  d4 |  r1 |  r2 |  r3 |  r4 | ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        StaticRoute = 33,

        #endregion 5. IP Layer Parameters per Interface

        #region 6. Link Layer Parameters per Interface

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  34 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        TrailerEncapsulation = 34,

        /// <summary>
        /// RFC 2132
        /// <pre>
        ///  Code   Len           Time
        /// +-----+-----+-----+-----+-----+-----+
        /// |  35 |  4  |  t1 |  t2 |  t3 |  t4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        ARPCacheTimeout = 35,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  36 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        EthernetEncapsulation = 36,

        #endregion 6. Link Layer Parameters per Interface

        #region 7. TCP Parameters

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  37 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        TCPDefaultTTL = 37,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len           Time
        /// +-----+-----+-----+-----+-----+-----+
        /// |  38 |  4  |  t1 |  t2 |  t3 |  t4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        TCPKeepaliveInterval = 38,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  39 |  1  | 0/1 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        TCPKeepaliveGarbage = 39,

        #endregion 7. TCP Parameters

        #region 8. Application and Service Parameters

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len      NIS Domain Name
        /// +-----+-----+-----+-----+-----+-----+---
        /// |  40 |  n  |  n1 |  n2 |  n3 |  n4 | ...
        /// +-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        NetworkInformationServiceDomain = 40,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  41 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        NetworkInformationServers = 41,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  42 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        NetworkTimeProtocolServers = 42,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        /// Code   Len   Vendor-specific information
        /// +-----+-----+-----+-----+---
        /// |  43 |  n  |  i1 |  i2 | ...
        /// +-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        VendorSpecificInformation = 43,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len           Address 1              Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
        /// |  44 |  n  |  a1 |  a2 |  a3 |  a4 |  b1 |  b2 |  b3 |  b4 | ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
        /// </pre>
        /// </summary>
        NetBIOSOverTCPIPNameServer = 44,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len           Address 1              Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
        /// |  45 |  n  |  a1 |  a2 |  a3 |  a4 |  b1 |  b2 |  b3 |  b4 | ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+----
        /// </pre>
        /// </summary>
        NetBIOSOverTCPIPDatagramDistributionServer = 45,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Node Type
        /// +-----+-----+-----------+
        /// |  46 |  1  |   flags   |
        /// +-----+-----+-----------+
        /// </pre>
        /// </summary>
        NetBIOSOverTCPIPNodeType = 46,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        /// Code   Len       NetBIOS Scope
        /// +-----+-----+-----+-----+-----+-----+----
        /// |  47 |  n  |  s1 |  s2 |  s3 |  s4 | ...
        /// +-----+-----+-----+-----+-----+-----+----
        /// </pre>
        /// </summary>
        NetBIOSOverTCPIPScope = 47,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// |  48 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |   ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        XWindowSystemFontServer = 48,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// |  49 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |   ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        XWindowSystemDisplayManager = 49,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len      NIS Client Domain Name
        /// +-----+-----+-----+-----+-----+-----+---
        /// |  64 |  n  |  n1 |  n2 |  n3 |  n4 | ...
        /// +-----+-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        NetworkInformationServicePlusDomain = 64,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// |  65 |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        NetworkInformationServicePlusServers = 65,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code Len    Home Agent Addresses (zero or more)
        /// +-----+-----+-----+-----+-----+-----+--
        /// | 68  |  n  | a1  | a2  | a3  | a4  | ...
        /// +-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        MobileIPHomeAgent = 68,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 69  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        SimpleMailTransportProtocolServer = 69,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 70  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        PostOfficeProtocolServer = 70,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 71  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        NetworkNewsTransportProtocolServer = 71,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 72  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        DefaultWorldWideWebServer = 72,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 73  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        DefaultFingerServer = 73,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 74  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        DefaultInternetRelayChatServer = 74,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 75  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        StreetTalkServer = 75,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Address 1               Address 2
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// | 76  |  n  |  a1 |  a2 |  a3 |  a4 |  a1 |  a2 |  ...
        /// +-----+-----+-----+-----+-----+-----+-----+-----+--
        /// </pre>
        /// </summary>
        StreetTalkDirectoryAssistanceServer = 76,

        #endregion 8. Application and Service Parameters

        #region 9. DHCP Extensions

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len          Address
        /// +-----+-----+-----+-----+-----+-----+
        /// |  50 |  4  |  a1 |  a2 |  a3 |  a4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        RequestedIPAddress = 50,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len         Lease Time
        /// +-----+-----+-----+-----+-----+-----+
        /// |  51 |  4  |  t1 |  t2 |  t3 |  t4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        IPAddressLeaseTime = 51,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Value
        /// +-----+-----+-----+
        /// |  52 |  1  |1/2/3|
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        OptionOverload = 52,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code Len    TFTP server
        /// +-----+-----+-----+-----+-----+---
        /// | 66  |  n  |  c1 |  c2 |  c3 | ...
        /// +-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        TFTPServerName = 66,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code Len    Bootfile name
        /// +-----+-----+-----+-----+-----+---
        /// | 67  |  n  |  c1 |  c2 |  c3 | ...
        /// +-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        BootfileName = 67,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len  Type
        /// +-----+-----+-----+
        /// |  53 |  1  | 1-7 |
        /// +-----+-----+-----+
        /// </pre>
        /// </summary>
        MessageType = 53,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len            Address
        /// +-----+-----+-----+-----+-----+-----+
        /// |  54 |  4  |  a1 |  a2 |  a3 |  a4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        ServerIdentifier = 54,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        /// Code   Len   Option Codes
        /// +-----+-----+-----+-----+---
        /// |  55 |  n  |  c1 |  c2 | ...
        /// +-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        ParameterRequestList = 55,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len     Text
        /// +-----+-----+-----+-----+---
        /// |  56 |  n  |  c1 |  c2 | ...
        /// +-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        Message = 56,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len     Length
        /// +-----+-----+-----+-----+
        /// |  57 |  2  |  l1 |  l2 |
        /// +-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        MaximumDhcpMessageSize = 57,

        /// <summary>
        /// RFC 2132
        /// <pre>
        ///  Code   Len         T1 Interval
        /// +-----+-----+-----+-----+-----+-----+
        /// |  58 |  4  |  t1 |  t2 |  t3 |  t4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        RenewalTimeValue = 58,

        /// <summary>
        /// RFC 2132
        /// <pre>
        ///  Code   Len         T1 Interval
        /// +-----+-----+-----+-----+-----+-----+
        /// |  5859 |  4  |  t1 |  t2 |  t3 |  t4 |
        /// +-----+-----+-----+-----+-----+-----+
        /// </pre>
        /// </summary>
        RebindingTimeValue = 59,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len   Vendor class Identifier
        /// +-----+-----+-----+-----+---
        /// |  60 |  n  |  i1 |  i2 | ...
        /// +-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        VendorClassidentifier = 60,

        /// <summary>
        /// RFC 2132.
        /// <pre>
        ///  Code   Len   Type  Client-Identifier
        /// +-----+-----+-----+-----+-----+---
        /// |  61 |  n  |  t1 |  i1 |  i2 | ...
        /// +-----+-----+-----+-----+-----+---
        /// </pre>
        /// </summary>
        ClientIdentifier = 61,

        #endregion 9. DHCP Extensions
    }
}