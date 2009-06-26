#include "PcapDeviceHandler.h"

#include "MarshalingServices.h"
#include "PcapDumpFile.h"
#include "Timestamp.h"
#include "PcapError.h"
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

DeviceHandlerResult PcapDeviceHandler::GetNextPacket([Out] Packet^% packet)
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

DeviceHandlerResult PcapDeviceHandler::GetNextPackets(int maxPackets, HandlePacket^ callBack, [Out] int% numPacketsGot)
{
    AssertMode(DeviceHandlerMode::Capture);

    PacketHandler^ packetHandler = gcnew PacketHandler(callBack);
    HandlerDelegate^ packetHandlerDelegate = gcnew HandlerDelegate(packetHandler, 
                                                                   &PacketHandler::Handle);

    return RunPcapDispatch(maxPackets, packetHandlerDelegate, numPacketsGot);
}

DeviceHandlerResult PcapDeviceHandler::GetNextStatistics([Out] PcapStatistics^% statistics)
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
PcapStatistics^ PcapDeviceHandler::CreateStatistics(const pcap_pkthdr& packetHeader, const unsigned char* packetData)
{
    DateTime timestamp;
    Timestamp::PcapTimestampToDateTime(packetHeader.ts, timestamp);

    unsigned long acceptedPackets = *reinterpret_cast<const unsigned long*>(packetData);
    unsigned long acceptedBytes = *reinterpret_cast<const unsigned long*>(packetData + 8);

    return gcnew PcapStatistics(timestamp, acceptedPackets, acceptedBytes);
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

DeviceHandlerResult PcapDeviceHandler::RunPcapDispatch(int maxInstances, HandlerDelegate^ callBack, [System::Runtime::InteropServices::Out] int% numInstancesGot)
{
    pcap_handler functionPointer = 
        (pcap_handler)Marshal::GetFunctionPointerForDelegate(callBack).ToPointer();

    numInstancesGot = pcap_dispatch(_pcapDescriptor, 
                                    maxInstances, 
                                    functionPointer,
                                    NULL);

    if (numInstancesGot == -1)
    {
        throw BuildInvalidOperation("Failed reading from device");
    }
    if (numInstancesGot == -2)
    {
        return DeviceHandlerResult::BreakLoop;
    }

    return DeviceHandlerResult::Ok;
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