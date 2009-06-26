#include "PcapDeviceHandler.h"

#include "MarshalingServices.h"
#include "PcapDumpFile.h"
#include "Timestamp.h"
#include "Pcap.h"

using namespace System;
using namespace BPacket;
using namespace PcapDotNet;
using namespace System::Runtime::InteropServices;

void packet_handler(u_char *param, const struct pcap_pkthdr *header, const u_char *pkt_data);

PcapDeviceHandler::PcapDeviceHandler(const char* source, int snapLen, PcapDeviceOpenFlags flags, int readTimeout, pcap_rmtauth *auth, SocketAddress^ netmask)
{
    // Open the device
    char errbuf[PCAP_ERRBUF_SIZE];
    pcap_t *pcapDescriptor = pcap_open(source,                // name of the device
                                       snapLen,               // portion of the packet to capture
                                                              // 65536 guarantees that the whole packet will be captured on all the link layers
                                       safe_cast<int>(flags),
                                       readTimeout,           // read timeout
                                       auth,                  // authentication on the remote machine
                                       errbuf);               // error buffer

    if (pcapDescriptor == NULL)
    {
        gcnew InvalidOperationException(String::Format("Unable to open the adapter. %s is not supported by WinPcap", gcnew String(source)));
    }

    _pcapDescriptor = pcapDescriptor;
    _ipV4Netmask = dynamic_cast<IpV4SocketAddress^>(netmask);
}

DeviceHandlerMode PcapDeviceHandler::Mode::get()
{
    return _mode;
}

void PcapDeviceHandler::Mode::set(DeviceHandlerMode value)
{
    if (pcap_setmode(_pcapDescriptor, safe_cast<int>(value)) < 0)
        throw gcnew InvalidOperationException("Error setting the mode.");
    _mode = value;
}

DeviceHandlerResult PcapDeviceHandler::GetNextPacket([Out] Packet^% packet)
{
    if (Mode != DeviceHandlerMode::Capture)
        throw gcnew InvalidOperationException("Must be in capture mode to get packets");

    pcap_pkthdr* packetHeader;
    const unsigned char* packetData;
    DeviceHandlerResult result = RunPcapNextEx(&packetHeader, &packetData);

    if (result != DeviceHandlerResult::Ok)
    {
        packet = nullptr;
        return result;
    }

    packet = CreatePacket(*packetHeader, packetData);
    return result;
}

DeviceHandlerResult PcapDeviceHandler::GetNextPackets(int maxPackets, HandlePacket^ callBack, [Out] int% numPacketsGot)
{
    if (Mode != DeviceHandlerMode::Capture)
        throw gcnew InvalidOperationException("Must be in capture mode to get packets");

    PacketHandler^ packetHandler = gcnew PacketHandler(callBack);
    PacketHandler::Delegate^ packetHandlerDelegate = gcnew PacketHandler::Delegate(packetHandler, 
                                                                                  &PacketHandler::Handle);
    pcap_handler functionPointer = 
        (pcap_handler)Marshal::GetFunctionPointerForDelegate(packetHandlerDelegate).ToPointer();

    numPacketsGot = pcap_dispatch(_pcapDescriptor, 
                                  maxPackets, 
                                  functionPointer,
                                  NULL);

    if (numPacketsGot == -1)
    {
        throw gcnew InvalidOperationException("Failed Getting packets. Error: " + gcnew String(pcap_geterr(_pcapDescriptor)));
    }
    if (numPacketsGot == -2)
    {
        return DeviceHandlerResult::BreakLoop;
    }

    return DeviceHandlerResult::Ok;
}

DeviceHandlerResult PcapDeviceHandler::GetNextStatistics([Out] PcapStatistics^% statistics)
{
    if (Mode != DeviceHandlerMode::Statistics)
        throw gcnew InvalidOperationException("Must be in statistics mode to get statistics");

    pcap_pkthdr* packetHeader;
    const unsigned char* packetData;
    DeviceHandlerResult result = RunPcapNextEx(&packetHeader, &packetData);

    if (result != DeviceHandlerResult::Ok)
    {
        statistics = nullptr;
        return result;
    }

    timeval pcapTimestamp = packetHeader->ts;
    DateTime timestamp;
    Timestamp::PcapTimestampToDateTime(packetHeader->ts, timestamp);

    unsigned long acceptedPackets = *reinterpret_cast<const unsigned long*>(packetData);
    unsigned long acceptedBytes = *reinterpret_cast<const unsigned long*>(packetData + 8);

    statistics = gcnew PcapStatistics(timestamp, acceptedPackets, acceptedBytes);

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

// static
Packet^ PcapDeviceHandler::CreatePacket(const pcap_pkthdr& packetHeader, const unsigned char* packetData)
{
    DateTime timestamp;
    Timestamp::PcapTimestampToDateTime(packetHeader.ts, timestamp);

    array<Byte>^ managedPacketData = MarshalingServices::UnamangedToManagedByteArray(packetData, 0, packetHeader.caplen);
    return gcnew Packet(managedPacketData, timestamp);
}

DeviceHandlerResult PcapDeviceHandler::RunPcapNextEx(pcap_pkthdr** packetHeader, const unsigned char** packetData)
{
    int result = pcap_next_ex(_pcapDescriptor, packetHeader, packetData);
    switch (result)
    {
    case -2: 
        return DeviceHandlerResult::Eof;
    case -1: 
        return DeviceHandlerResult::Error;
    case 0: 
        return DeviceHandlerResult::Timeout;
    case 1: 
        return DeviceHandlerResult::Ok;
    default: 
        throw gcnew InvalidOperationException("Result value " + result + " is undefined");
    }
}

void PcapDeviceHandler::PacketHandler::Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData)
{
    _callBack->Invoke(CreatePacket(*packetHeader, packetData));
}