#pragma once

#include "PcapAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet 
{
    public ref class BpfFilter : System::IDisposable
    {
    public:
        BpfFilter(pcap_t* handler, System::String^ filterString, IpV4SocketAddress^ netmask);

        ~BpfFilter(); // IDisposable

        property bpf_program& Bpf
        {
            bpf_program& get();
        }

    private:
        bpf_program* _bpf;
    };
}