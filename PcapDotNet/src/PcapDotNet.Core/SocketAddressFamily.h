#pragma once

namespace PcapDotNet { namespace Core 
{
    public enum class SocketAddressFamily : System::UInt16
    {
        Unspecified = 0,               // unspecified
        Unix        = 1,               // local to host (pipes, portals)
        Internet    = 2,               // internetwork: UDP, TCP, etc.
        ImpLink     = 3,               // arpanet imp addresses
        Pup         = 4,               // pup protocols: e.g. BSP
        Chaos       = 5,               // mit CHAOS protocols
        NS          = 6,               // XEROX NS protocols
        Ipx         = NS,              // IPX protocols: IPX, SPX, etc.
        Iso         = 7,               // ISO protocols
        Osi         = Iso,             // OSI is ISO
        EuropeanComputerManufactures = 8,               // european computer manufacturers
        Datakit     = 9,               // datakit protocols
        Ccitt       = 10,         
        // CCITT protocols, X.25 etc
        Sna         = 11,              // IBM SNA
        DECnet      = 12,              // DECnet
        DirectDataLinkInterface = 13,              // Direct data link interface
        Lat         = 14,              // LAT
        HyperChannel = 15,             // NSC Hyperchannel
        AppleTalk   = 16,              // AppleTalk
        NetBios     = 17,              // NetBios-style addresses
        VoiceView   = 18,              // VoiceView
        Firefox     = 19,              // Protocols from Firefox
        Unknown1    = 20,              // Somebody is using this!
        Ban         = 21,              // Banyan
        Atm         = 22,              // Native ATM Services
        Internet6   = 23,              // Internetwork Version 6
        Cluster     = 24,              // Microsoft Wolfpack
        Ieee12844   = 25,              // IEEE 1284.4 WG AF
        Irda        = 26,              // IrDA
        NetworkDesigners = 28,              // Network Designers OSI & gateway
        TcnProcess  = 29,
        TcnMessage  = 30,
        Iclfxbm     = 31,
        Bluetooth   = 32               // Bluetooth RFCOMM/L2CAP protocols
    };
}}