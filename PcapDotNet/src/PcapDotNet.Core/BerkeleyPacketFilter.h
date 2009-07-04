#pragma once

#include "IpV4socketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class BerkeleyPacketFilter : System::IDisposable
    {
    public:
        void SetFilter(pcap_t* pcapDescriptor);

        ~BerkeleyPacketFilter(); // IDisposable

    internal:
        BerkeleyPacketFilter(pcap_t* pcapDescriptor, System::String^ filterString, IpV4SocketAddress^ netmask);

    private:
        bpf_program* _bpf;
    };
}}