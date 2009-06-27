#include "BpfFilter.h"
#include "Pcap.h"
#include "MarshalingServices.h"
#include "PcapError.h"

using namespace System;
using namespace PcapDotNet::Core;

BpfFilter::BpfFilter(pcap_t* pcapDescriptor, String^ filterString, IpV4SocketAddress^ netmask)
{
    std::string unmanagedFilterString = MarshalingServices::ManagedToUnmanagedString(filterString);

    unsigned int netmaskValue = 0;
    if (netmask != nullptr)
        netmaskValue = netmask->Address;

    _bpf = new bpf_program();

    try
    {
        if (pcap_compile(pcapDescriptor, _bpf, const_cast<char*>(unmanagedFilterString.c_str()), 1, netmaskValue) < 0)
        {
            gcnew ArgumentException("An error has occured when compiling the filter: " + PcapError::GetErrorMessage(pcapDescriptor));
        }
    }
    catch(...)
    {
        delete _bpf;
        throw;
    }
}

void BpfFilter::SetFilter(pcap_t* pcapDescriptor)
{
    if (pcap_setfilter(pcapDescriptor, _bpf) != 0)
        throw PcapError::BuildInvalidOperation("Failed setting bpf filter", pcapDescriptor);
}


BpfFilter::~BpfFilter()
{
    pcap_freecode(_bpf);
    delete _bpf;
}
