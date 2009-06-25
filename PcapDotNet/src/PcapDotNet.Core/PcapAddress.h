#pragma once

struct sockaddr;
typedef struct pcap_addr pcap_addr_t;

namespace PcapDotNet 
{
    public enum class SocketAddressFamily : System::UInt16
    {
        UNSPEC      = 0,               // unspecified
        UNIX        = 1,               // local to host (pipes, portals)
        INET        = 2,               // internetwork: UDP, TCP, etc.
        IMPLINK     = 3,               // arpanet imp addresses
        PUP         = 4,               // pup protocols: e.g. BSP
        CHAOS       = 5,               // mit CHAOS protocols
        NS          = 6,               // XEROX NS protocols
        IPX         = NS,              // IPX protocols: IPX, SPX, etc.
        ISO         = 7,               // ISO protocols
        OSI         = ISO,             // OSI is ISO
        ECMA        = 8,               // european computer manufacturers
        DATAKIT     = 9,               // datakit protocols
        CCITT       = 10,              // CCITT protocols, X.25 etc
        SNA         = 11,              // IBM SNA
        DECnet      = 12,              // DECnet
        DLI         = 13,              // Direct data link interface
        LAT         = 14,              // LAT
        HYLINK      = 15,              // NSC Hyperchannel
        APPLETALK   = 16,              // AppleTalk
        NETBIOS     = 17,              // NetBios-style addresses
        VOICEVIEW   = 18,              // VoiceView
        FIREFOX     = 19,              // Protocols from Firefox
        UNKNOWN1    = 20,              // Somebody is using this!
        BAN         = 21,              // Banyan
        ATM         = 22,              // Native ATM Services
        INET6       = 23,              // Internetwork Version 6
        CLUSTER     = 24,              // Microsoft Wolfpack
        IEEE12844   = 25,              // IEEE 1284.4 WG AF
        IRDA        = 26,              // IrDA
        NETDES      = 28,              // Network Designers OSI & gateway
        TCNPROCESS  = 29,
        TCNMESSAGE  = 30,
        ICLFXBM     = 31,
        BTH         = 32               // Bluetooth RFCOMM/L2CAP protocols
    };

    public ref class SocketAddress
    {
    public:
        SocketAddress(unsigned short family);

        property SocketAddressFamily^ Family
        {
            SocketAddressFamily^ get();
        }

        virtual System::String^ ToString() override;

    private:
        SocketAddressFamily^ _family;
    };

    public ref class IpV4SocketAddress : SocketAddress
    {
    public:
        IpV4SocketAddress(sockaddr *address);

        property unsigned int Address
        {
            unsigned int get();
        }

        property System::String^ AddressString
        {
            System::String^ get();
        }

        virtual System::String^ ToString() override;

    private:
        unsigned int _address;
    };

    public ref class PcapAddress
    {
    public:
        PcapAddress(pcap_addr_t *pcapAddress);

        property SocketAddress^ Address
        {
            SocketAddress^ get();
        }

        property SocketAddress^ Netmask
        {
            SocketAddress^ get();
        }

        property SocketAddress^ Broadcast
        {
            SocketAddress^ get();
        }

        property SocketAddress^ Destination
        {
            SocketAddress^ get();
        }

        virtual System::String^ ToString() override;

    private:
        SocketAddress^ _address;
        SocketAddress^ _netmask;
        SocketAddress^ _broadcast;
        SocketAddress^ _destination;
    };
}