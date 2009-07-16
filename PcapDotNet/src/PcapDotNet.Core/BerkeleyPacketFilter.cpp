#include "BerkeleyPacketFilter.h"
#include "Pcap.h"
#include "MarshalingServices.h"
#include "PcapError.h"
#include "PcapDataLink.h"
#include "PacketHeader.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace PcapDotNet::Core;
using namespace Packets;

BerkeleyPacketFilter::BerkeleyPacketFilter(String^ filterValue, int snapshotLength, DataLinkKind kind, IpV4SocketAddress^ netmask)
{
    Initialize(filterValue, snapshotLength, kind, netmask);
}

BerkeleyPacketFilter::BerkeleyPacketFilter(String^ filterValue, int snapshotLength, DataLinkKind kind)
{
    Initialize(filterValue, snapshotLength, kind, nullptr);
}

bool BerkeleyPacketFilter::Test([Out] int% snapshotLength, Packets::Packet^ packet)
{
    pcap_pkthdr pcapHeader;
    PacketHeader::GetPcapHeader(pcapHeader, packet);
    pin_ptr<Byte> unmanagedPacketBytes = &packet->Buffer[0];

    snapshotLength = pcap_offline_filter(_bpf, &pcapHeader, unmanagedPacketBytes);
    return (snapshotLength != 0);
}

bool BerkeleyPacketFilter::Test(Packets::Packet^ packet)
{
    int snapshotLength;
    return Test(snapshotLength, packet);
}

BerkeleyPacketFilter::~BerkeleyPacketFilter()
{
    pcap_freecode(_bpf);
    delete _bpf;
}

// Internal

BerkeleyPacketFilter::BerkeleyPacketFilter(pcap_t* pcapDescriptor, String^ filterString, IpV4SocketAddress^ netmask)
{
    Initialize(pcapDescriptor, filterString, netmask);
}

void BerkeleyPacketFilter::SetFilter(pcap_t* pcapDescriptor)
{
    if (pcap_setfilter(pcapDescriptor, _bpf) != 0)
        throw PcapError::BuildInvalidOperation("Failed setting bpf filter", pcapDescriptor);
}

// Private

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
            throw gcnew ArgumentException("An error has occured when compiling the filter <" + filterString + ">: " + PcapError::GetErrorMessage(pcapDescriptor));
        }
    }
    catch(...)
    {
        delete _bpf;
        throw;
    }
}
