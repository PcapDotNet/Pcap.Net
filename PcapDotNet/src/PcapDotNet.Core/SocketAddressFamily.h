#pragma once

namespace PcapDotNet { namespace Core 
{
	/// <summary>
	/// The type of socket address for a device address.
	/// </summary>
    public enum class SocketAddressFamily : System::UInt16
    {
		/// <summary>
		/// unspecified
		/// </summary>
        Unspecified                  = 0,

		/// <summary>
		/// local to host (pipes, portals)
		/// </summary>
		Unix                         = 1,

		/// <summary>
		/// internetwork: UDP, TCP, etc.
		/// </summary>
        Internet                     = 2,

		/// <summary>
		/// arpanet imp addresses
		/// </summary>
        ImpLink                      = 3,

		/// <summary>
		/// pup protocols: e.g. BSP
		/// </summary>
        Pup                          = 4,

		/// <summary>
		/// mit CHAOS protocols
		/// </summary>
        Chaos                        = 5,

		/// <summary>
		/// XEROX NS protocols
		/// </summary>
        NS                           = 6,

		/// <summary>
		/// IPX protocols: IPX, SPX, etc.
		/// </summary>
        Ipx                          = NS,

		/// <summary>
		/// ISO protocols
		/// </summary>
        Iso                          = 7,

		/// <summary>
		/// OSI is ISO
		/// </summary>
        Osi                          = Iso,

		/// <summary>
		/// european computer manufacturers
		/// </summary>
        EuropeanComputerManufactures = 8,

		/// <summary>
		/// datakit protocols
		/// </summary>
        Datakit                      = 9,

		/// <summary>
		/// CCITT protocols, X.25 etc
		/// </summary>
        Ccitt                        = 10,         

		/// <summary>
		/// IBM SNA
		/// </summary>
        Sna                          = 11,

		/// <summary>
		/// DECnet
		/// </summary>
        DECnet                       = 12,

		/// <summary>
		/// Direct data link interface
		/// </summary>
        DirectDataLinkInterface      = 13,

		/// <summary>
		/// LAT
		/// </summary>
        Lat                          = 14,

		/// <summary>
		/// NSC Hyperchannel
		/// </summary>
        HyperChannel                 = 15,

		/// <summary>
		/// AppleTalk
		/// </summary>
        AppleTalk                    = 16,

		/// <summary>
		/// NetBios-style addresses
		/// </summary>
        NetBios                      = 17,

		/// <summary>
		/// VoiceView
		/// </summary>
        VoiceView                    = 18,

		/// <summary>
		/// Protocols from Firefox
		/// </summary>
        Firefox                      = 19,

		/// <summary>
		/// Somebody is using this!
		/// </summary>
        Unknown1                     = 20,

		/// <summary>
		/// Banyan
		/// </summary>
        Ban                          = 21,

		/// <summary>
		/// Native ATM Services
		/// </summary>
        Atm                          = 22,

		/// <summary>
		/// Internetwork Version 6
		/// </summary>
        Internet6                    = 23,

		/// <summary>
		/// Microsoft Wolfpack
		/// </summary>
        Cluster                      = 24,

		/// <summary>
		/// IEEE 1284.4 WG AF
		/// </summary>
        Ieee12844                    = 25,

		/// <summary>
		/// IrDA
		/// </summary>
        Irda                         = 26,

		/// <summary>
		/// Network Designers OSI &amp; gateway
		/// </summary>
        NetworkDesigners             = 28,

        TcnProcess                   = 29,

		TcnMessage                   = 30,

		Iclfxbm                      = 31,

		/// <summary>
		/// Bluetooth RFCOMM/L2CAP protocols
		/// </summary>
        Bluetooth                    = 32
    };
}}