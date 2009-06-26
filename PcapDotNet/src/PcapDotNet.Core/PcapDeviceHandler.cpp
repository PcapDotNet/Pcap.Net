#include "PcapDeviceHandler.h"

#include "MarshalingServices.h"
#include "PcapDumpFile.h"
#include "Timestamp.h"
#include "Pcap.h"

using namespace System;
using namespace BPacket;
using namespace PcapDotNet;

PcapDeviceHandler::PcapDeviceHandler(pcap_t* pcapDescriptor, SocketAddress^ netmask)
{
    _pcapDescriptor = pcapDescriptor;
    _ipV4Netmask = dynamic_cast<IpV4SocketAddress^>(netmask);
}

DeviceHandlerResult PcapDeviceHandler::GetNextPacket([System::Runtime::InteropServices::Out] Packet^% packet)
{
    packet = nullptr;

    pcap_pkthdr* packetHeader;
    const unsigned char* packetData;
    DeviceHandlerResult result = safe_cast<DeviceHandlerResult>(pcap_next_ex(_pcapDescriptor, &packetHeader, &packetData));

    if (result != DeviceHandlerResult::Ok)
        return result;

    timeval pcapTimestamp = packetHeader->ts;
    DateTime timestamp;
    Timestamp::PcapTimestampToDateTime(packetHeader->ts, timestamp);

    array<Byte>^ managedPacketData = MarshalingServices::UnamangedToManagedByteArray(packetData, 0, packetHeader->caplen);
    packet = gcnew Packet(managedPacketData, timestamp);

    return result;
}

void PcapDeviceHandler::SendPacket(Packet^ packet)
{
    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    if (pcap_sendpacket(_pcapDescriptor, unamangedPacketBytes, packet->Length) != 0)
    {
        throw gcnew InvalidOperationException("Failed sending packet");
    }
}

BpfFilter^ PcapDeviceHandler::CreateFilter(String^ filterString)
{
    return gcnew BpfFilter(_pcapDescriptor, filterString, _ipV4Netmask);
}

void PcapDeviceHandler::SetFilter(BpfFilter^ filter)
{
    if (pcap_setfilter(_pcapDescriptor, &filter->Bpf) < 0)
    {
        throw gcnew InvalidOperationException("Error setting the filter: " + gcnew String(pcap_geterr(_pcapDescriptor)));
    }
}

void PcapDeviceHandler::SetFilter(String^ filterString)
{
    BpfFilter^ filter = CreateFilter(filterString);
    try
    {
        SetFilter(filter);
    }
    finally
    {
        filter->~BpfFilter();
    }
}

PcapDumpFile^ PcapDeviceHandler::OpenDump(System::String^ filename)
{
    std::string unmanagedString = MarshalingServices::ManagedToUnmanagedString(filename);
    pcap_dumper_t* dumpFile = pcap_dump_open(_pcapDescriptor, unmanagedString.c_str());
    if (dumpFile == NULL)
        throw gcnew InvalidOperationException("Error opening output file " + filename + " Error: " + gcnew System::String(pcap_geterr(_pcapDescriptor)));
    return gcnew PcapDumpFile(dumpFile, filename);
}

PcapDeviceHandler::~PcapDeviceHandler()
{
    pcap_close(_pcapDescriptor);
}

pcap_t* PcapDeviceHandler::Descriptor::get()
{
    return _pcapDescriptor;
}