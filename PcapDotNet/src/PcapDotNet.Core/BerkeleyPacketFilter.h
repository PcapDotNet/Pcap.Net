#pragma once

#include "IpV4socketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class BerkeleyPacketFilter : System::IDisposable
    {
    public:
        BerkeleyPacketFilter(System::String^ filterString, int snapshotLength, Packets::DataLinkKind kind, IpV4SocketAddress^ netmask);
        BerkeleyPacketFilter(System::String^ filterString, int snapshotLength, Packets::DataLinkKind kind);

        void SetFilter(pcap_t* pcapDescriptor);

        ~BerkeleyPacketFilter(); // IDisposable

    internal:
        BerkeleyPacketFilter(pcap_t* pcapDescriptor, System::String^ filterString, IpV4SocketAddress^ netmask);

    private:
        void Initialize(System::String^ filterString, int snapshotLength, Packets::DataLinkKind kind, IpV4SocketAddress^ netmask);
        void Initialize(pcap_t* pcapDescriptor, System::String^ filterString, IpV4SocketAddress^ netmask);

    private:
        bpf_program* _bpf;
    };
}}