#include "PcapDeviceHandler.h"

#include "MarshalingServices.h"
#include "PcapDumpFile.h"
#include "Timestamp.h"
#include "PcapError.h"
#include "Pcap.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::ObjectModel;
using namespace System::Collections::Generic;
using namespace BPacket;
using namespace PcapDotNet;

void packet_handler(u_char *param, const struct pcap_pkthdr *header, const u_char *pkt_data);

PcapDeviceHandler::PcapDeviceHandler(const char* source, int snapshotLength, PcapDeviceOpenFlags flags, int readTimeout, pcap_rmtauth *auth, SocketAddress^ netmask)
{
    // Open the device
    char errbuf[PCAP_ERRBUF_SIZE];
    pcap_t *pcapDescriptor = pcap_open(source,                // name of the device
                                       snapshotLength,               // portion of the packet to capture
                                                              // 65536 guarantees that the whole packet will be captured on all the link layers
                                       safe_cast<int>(flags),
                                       readTimeout,           // read timeout
                                       auth,                  // authentication on the remote machine
                                       errbuf);               // error buffer

    if (pcapDescriptor == NULL)
    {
        throw gcnew InvalidOperationException(String::Format("Unable to open the adapter. %s is not supported by WinPcap", gcnew String(source)));
    }

    _pcapDescriptor = pcapDescriptor;
    _ipV4Netmask = dynamic_cast<IpV4SocketAddress^>(netmask);
}

PcapDataLink PcapDeviceHandler::DataLink::get()
{
    return PcapDataLink(pcap_datalink(_pcapDescriptor));
}

void PcapDeviceHandler::DataLink::set(PcapDataLink value)
{
    if (pcap_set_datalink(_pcapDescriptor, value.Value) == -1)
        throw BuildInvalidOperation("Failed setting datalink " + value.ToString());
}

ReadOnlyCollection<PcapDataLink>^ PcapDeviceHandler::SupportedDataLinks::get()
{
    throw gcnew NotSupportedException("Supported DataLinks is unsupported to avoid winpcap memory leak");

    int* dataLinks;
    int numDatalinks = pcap_list_datalinks(_pcapDescriptor, &dataLinks);
    if (numDatalinks == -1)
        throw BuildInvalidOperation("Failed getting supported datalinks");

    try
    {
        List<PcapDataLink>^ results = gcnew List<PcapDataLink>(numDatalinks);
        for (int i = 0; i != numDatalinks; ++i)
            results->Add(PcapDataLink(dataLinks[i]));
        return gcnew ReadOnlyCollection<PcapDataLink>(results);
    }
    finally
    {
        // This doesn't work because of a bug. See http://www.winpcap.org/pipermail/winpcap-users/2008-May/002500.html
        // todo look for pcap_free_datalinks()
        // free(dataLinks);
    }
}

int PcapDeviceHandler::SnapshotLength::get()
{
    return pcap_snapshot(_pcapDescriptor);
}

bool PcapDeviceHandler::IsFileSystemByteOrder::get()
{
    return (pcap_is_swapped(_pcapDescriptor) == 0);
}
 
int PcapDeviceHandler::FileMajorVersion::get()
{
    return pcap_major_version(_pcapDescriptor);
}

int PcapDeviceHandler::FileMinorVersion::get()
{
    return pcap_minor_version(_pcapDescriptor);
}

PcapTotalStatistics^ PcapDeviceHandler::TotalStatistics::get()
{
    int statisticsSize;
    pcap_stat* statistics = pcap_stats_ex(_pcapDescriptor, &statisticsSize);
    if (statistics == NULL)
        throw BuildInvalidOperation("Failed getting total statistics");

    unsigned int packetsReceived = statistics->ps_recv;
    unsigned int packetsDroppedByDriver = statistics->ps_drop;
    unsigned int packetsDroppedByInterface = statistics->ps_ifdrop;
    unsigned int packetsCaptured = (statisticsSize >= 16 
                                        ? *(reinterpret_cast<int*>(statistics) + 3)
                                        : 0);
    return gcnew PcapTotalStatistics(packetsReceived, packetsDroppedByDriver, packetsDroppedByInterface, packetsCaptured);
}


DeviceHandlerMode PcapDeviceHandler::Mode::get()
{
    return _mode;
}

void PcapDeviceHandler::Mode::set(DeviceHandlerMode value)
{
    if (pcap_setmode(_pcapDescriptor, safe_cast<int>(value)) < 0)
        throw BuildInvalidOperation("Error setting mode " + value.ToString());
    _mode = value;
}

bool PcapDeviceHandler::NonBlocking::get()
{
    char errbuf[PCAP_ERRBUF_SIZE];
    int nonBlockValue = pcap_getnonblock(_pcapDescriptor, errbuf);
    if (nonBlockValue == -1)
        throw gcnew InvalidOperationException("Error getting NonBlocking value");
    return nonBlockValue != 0;
}

void PcapDeviceHandler::NonBlocking::set(bool value)
{
    char errbuf[PCAP_ERRBUF_SIZE];
    if (pcap_setnonblock(_pcapDescriptor, value, errbuf) != 0)
        throw gcnew InvalidOperationException("Error setting NonBlocking to " + value.ToString());
}

DeviceHandlerResult PcapDeviceHandler::GetPacket([Out] Packet^% packet)
{
    AssertMode(DeviceHandlerMode::Capture);

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

DeviceHandlerResult PcapDeviceHandler::GetSomePackets(int maxPackets, HandlePacket^ callBack, [Out] int% numPacketsGot)
{
    AssertMode(DeviceHandlerMode::Capture);

    PacketHandler^ packetHandler = gcnew PacketHandler(callBack);
    HandlerDelegate^ packetHandlerDelegate = gcnew HandlerDelegate(packetHandler, &PacketHandler::Handle);
    pcap_handler functionPointer = (pcap_handler)Marshal::GetFunctionPointerForDelegate(packetHandlerDelegate).ToPointer();

    numPacketsGot = pcap_dispatch(_pcapDescriptor, 
                                  maxPackets, 
                                  functionPointer,
                                  NULL);

    if (numPacketsGot == -1)
        throw BuildInvalidOperation("Failed reading from device");
    if (numPacketsGot == -2)
        return DeviceHandlerResult::BreakLoop;
    return DeviceHandlerResult::Ok;
}

DeviceHandlerResult PcapDeviceHandler::GetPackets(int numPackets, HandlePacket^ callBack)
{
    AssertMode(DeviceHandlerMode::Capture);

    PacketHandler^ packetHandler = gcnew PacketHandler(callBack);
    HandlerDelegate^ packetHandlerDelegate = gcnew HandlerDelegate(packetHandler, &PacketHandler::Handle);
    pcap_handler functionPointer = (pcap_handler)Marshal::GetFunctionPointerForDelegate(packetHandlerDelegate).ToPointer();

    int result = pcap_loop(_pcapDescriptor, numPackets, functionPointer, NULL);
    if (result == -1)
        throw BuildInvalidOperation("Failed reading from device");
    if (result == -2)
        return DeviceHandlerResult::BreakLoop;
    return DeviceHandlerResult::Ok;
}

DeviceHandlerResult PcapDeviceHandler::GetNextStatistics([Out] PcapSampleStatistics^% statistics)
{
    AssertMode(DeviceHandlerMode::Statistics);

    pcap_pkthdr* packetHeader;
    const unsigned char* packetData;
    DeviceHandlerResult result = RunPcapNextEx(&packetHeader, &packetData);

    if (result != DeviceHandlerResult::Ok)
    {
        statistics = nullptr;
        return result;
    }

    statistics = CreateStatistics(*packetHeader, packetData);

    return result;
}

void PcapDeviceHandler::SendPacket(Packet^ packet)
{
    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    if (pcap_sendpacket(_pcapDescriptor, unamangedPacketBytes, packet->Length) != 0)
        throw BuildInvalidOperation("Failed writing to device");
}

BpfFilter^ PcapDeviceHandler::CreateFilter(String^ filterString)
{
    return gcnew BpfFilter(_pcapDescriptor, filterString, _ipV4Netmask);
}

void PcapDeviceHandler::SetFilter(BpfFilter^ filter)
{
    filter->SetFilter(_pcapDescriptor);
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
    return gcnew PcapDumpFile(_pcapDescriptor, filename);
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

// static
PcapSampleStatistics^ PcapDeviceHandler::CreateStatistics(const pcap_pkthdr& packetHeader, const unsigned char* packetData)
{
    DateTime timestamp;
    Timestamp::PcapTimestampToDateTime(packetHeader.ts, timestamp);

    unsigned long acceptedPackets = *reinterpret_cast<const unsigned long*>(packetData);
    unsigned long acceptedBytes = *reinterpret_cast<const unsigned long*>(packetData + 8);

    return gcnew PcapSampleStatistics(timestamp, acceptedPackets, acceptedBytes);
}

DeviceHandlerResult PcapDeviceHandler::RunPcapNextEx(pcap_pkthdr** packetHeader, const unsigned char** packetData)
{
    int result = pcap_next_ex(_pcapDescriptor, packetHeader, packetData);
    switch (result)
    {
    case -2: 
        return DeviceHandlerResult::Eof;
    case -1: 
        throw PcapError::BuildInvalidOperation("Failed reading from device", _pcapDescriptor);
    case 0: 
        return DeviceHandlerResult::Timeout;
    case 1: 
        return DeviceHandlerResult::Ok;
    default: 
        throw gcnew InvalidOperationException("Result value " + result.ToString() + " is undefined");
    }
}

void PcapDeviceHandler::AssertMode(DeviceHandlerMode mode)
{
    if (Mode != mode)
        throw gcnew InvalidOperationException("Wrong Mode. Must be in mode " + mode.ToString() + " and not in mode " + Mode.ToString());
}

String^ PcapDeviceHandler::ErrorMessage::get()
{
    return PcapError::GetErrorMessage(_pcapDescriptor);
}

InvalidOperationException^ PcapDeviceHandler::BuildInvalidOperation(System::String^ errorMessage)
{
    return PcapError::BuildInvalidOperation(errorMessage, _pcapDescriptor);
}

void PcapDeviceHandler::PacketHandler::Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData)
{
    _callBack->Invoke(CreatePacket(*packetHeader, packetData));
}

void PcapDeviceHandler::StatisticsHandler::Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData)
{
    _callBack->Invoke(CreateStatistics(*packetHeader, packetData));
}