#pragma once

#include "IpV4socketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class BerkeleyPacketFilter : System::IDisposable
    {
    public:
        BerkeleyPacketFilter(pcap_t* pcapDescriptor, System::String^ filterString, IpV4SocketAddress^ netmask);

        void SetFilter(pcap_t* pcapDescriptor);

        ~BerkeleyPacketFilter(); // IDisposable

    private:
        bpf_program* _bpf;
    };
}}