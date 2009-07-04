#include "BerkeleyPacketFilter.h"
#include "Pcap.h"
#include "MarshalingServices.h"
#include "PcapError.h"
#include "PcapDataLink.h"

using namespace System;
using namespace PcapDotNet::Core;
using namespace Packets;

BerkeleyPacketFilter::BerkeleyPacketFilter(String^ filterString, int snapshotLength, DataLinkKind kind, IpV4SocketAddress^ netmask)
{
    Initialize(filterString, snapshotLength, kind, netmask);
}

BerkeleyPacketFilter::BerkeleyPacketFilter(String^ filterString, int snapshotLength, DataLinkKind kind)
{
    Initialize(filterString, snapshotLength, kind, nullptr);
}

BerkeleyPacketFilter::BerkeleyPacketFilter(pcap_t* pcapDescriptor, String^ filterString, IpV4SocketAddress^ netmask)
{
    Initialize(pcapDescriptor, filterString, netmask);
}

void BerkeleyPacketFilter::SetFilter(pcap_t* pcapDescriptor)
{
    if (pcap_setfilter(pcapDescriptor, _bpf) != 0)
        throw PcapError::BuildInvalidOperation("Failed setting bpf filter", pcapDescriptor);
}

BerkeleyPacketFilter::~BerkeleyPacketFilter()
{
    pcap_freecode(_bpf);
    delete _bpf;
}

void BerkeleyPacketFilter::Initialize(String^ filterString, int snapshotLength, DataLinkKind kind, IpV4SocketAddress^ netmask)
{
    PcapDataLink dataLink = PcapDataLink(kind);
    pcap_t* pcapDescriptor = pcap_open_dead(dataLink.Value, snapshotLength);
    try
    {
        Initialize(pcapDescriptor, filterString, netmask);
    }
    finally
    {
        pcap_close(pcapDescriptor);
    }
}

void BerkeleyPacketFilter::Initialize(pcap_t* pcapDescriptor, String^ filterString, IpV4SocketAddress^ netmask)
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
