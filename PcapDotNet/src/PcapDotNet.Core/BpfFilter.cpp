#include "BpfFilter.h"
#include "Pcap.h"
#include "MarshalingServices.h"

using namespace System;
using namespace PcapDotNet;

BpfFilter::BpfFilter(pcap_t* handler, String^ filterString, IpV4SocketAddress^ netmask)
{
    std::string unmanagedFilterString = MarshalingServices::ManagedToUnmanagedString(filterString);

    unsigned int netmaskValue = 0;
    if (netmask != nullptr)
        netmaskValue = netmask->Address;

    _bpf = new bpf_program();

    try
    {
        if (pcap_compile(handler, _bpf, const_cast<char*>(unmanagedFilterString.c_str()), 1, netmaskValue) < 0)
        {
            gcnew ArgumentException("An error has occured when compiling the filter: " + gcnew String(pcap_geterr(handler)));
        }
    }
    catch(...)
    {
        delete _bpf;
        throw;
    }
}

BpfFilter::~BpfFilter()
{
    delete _bpf;
}

bpf_program& BpfFilter::Bpf::get()
{
    return *_bpf;
}
