#pragma once

#include "IpV4socketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class BpfFilter : System::IDisposable
    {
    public:
        BpfFilter(pcap_t* pcapDescriptor, System::String^ filterString, IpV4SocketAddress^ netmask);

        void SetFilter(pcap_t* pcapDescriptor);

        ~BpfFilter(); // IDisposable

    private:
        bpf_program* _bpf;
    };
}}