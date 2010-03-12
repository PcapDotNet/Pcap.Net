#include "PacketCommunicator.h"

#include "MarshalingServices.h"
#include "PacketDumpFile.h"
#include "PacketTimestamp.h"
#include "PcapError.h"
#include "Pcap.h"

using namespace System;
using namespace System::Globalization;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::ObjectModel;
using namespace System::Collections::Generic;
using namespace PcapDotNet::Packets;
using namespace PcapDotNet::Core;

PcapDataLink PacketCommunicator::DataLink::get()
{
    return PcapDataLink(pcap_datalink(_pcapDescriptor));
}

void PacketCommunicator::DataLink::set(PcapDataLink value)
{
    if (pcap_set_datalink(_pcapDescriptor, value.Value) == -1)
        throw BuildInvalidOperation("Failed setting datalink " + value.ToString());
}

ReadOnlyCollection<PcapDataLink>^ PacketCommunicator::SupportedDataLinks::get()
{
    throw gcnew NotSupportedException("Supported DataLinks is unsupported to avoid winpcap memory leak");
//	pcap_free_datalinks(NULL);
/*
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
    */
}

int PacketCommunicator::SnapshotLength::get()
{
    return pcap_snapshot(_pcapDescriptor);
}

IpV4SocketAddress^ PacketCommunicator::IpV4Netmask::get()
{
    return _ipV4Netmask;
}

bool PacketCommunicator::IsFileSystemByteOrder::get()
{
    return (pcap_is_swapped(_pcapDescriptor) == 0);
}
 
int PacketCommunicator::FileMajorVersion::get()
{
    return pcap_major_version(_pcapDescriptor);
}

int PacketCommunicator::FileMinorVersion::get()
{
    return pcap_minor_version(_pcapDescriptor);
}

PacketCommunicatorMode PacketCommunicator::Mode::get()
{
    return _mode;
}

void PacketCommunicator::Mode::set(PacketCommunicatorMode value)
{
    if (pcap_setmode(_pcapDescriptor, safe_cast<int>(value)) < 0)
        throw BuildInvalidOperation("Error setting mode " + value.ToString());
    _mode = value;
}

bool PacketCommunicator::NonBlocking::get()
{
    char errorBuffer[PCAP_ERRBUF_SIZE];
    int nonBlockValue = pcap_getnonblock(_pcapDescriptor, errorBuffer);
    if (nonBlockValue == -1)
        throw BuildInvalidOperation("Error getting NonBlocking value");
    return nonBlockValue != 0;
}

void PacketCommunicator::NonBlocking::set(bool value)
{
    char errorBuffer[PCAP_ERRBUF_SIZE];
    if (pcap_setnonblock(_pcapDescriptor, value, errorBuffer) != 0)
        throw BuildInvalidOperation("Error setting NonBlocking to " + value.ToString());
}

void PacketCommunicator::SetKernelBufferSize(int size)
{
    if (pcap_setbuff(_pcapDescriptor, size) != 0)
        throw BuildInvalidOperation("Error setting kernel buffer size to " + size.ToString(CultureInfo::InvariantCulture));
}

void PacketCommunicator::SetKernelMinimumBytesToCopy(int size)
{
    if (pcap_setmintocopy(_pcapDescriptor, size) != 0)
        throw BuildInvalidOperation("Error setting kernel minimum bytes to copy to " + size.ToString(CultureInfo::InvariantCulture));
}

void PacketCommunicator::SetSamplingMethod(SamplingMethod^ method)
{
    pcap_samp* pcapSamplingMethod = pcap_setsampling(_pcapDescriptor);
    pcapSamplingMethod->method = method->Method;
    pcapSamplingMethod->value = method->Value;
}

PacketCommunicatorReceiveResult PacketCommunicator::ReceivePacket([Out] Packet^% packet)
{
    AssertMode(PacketCommunicatorMode::Capture);

    pcap_pkthdr* packetHeader;
    const unsigned char* packetData;
    PacketCommunicatorReceiveResult result = RunPcapNextEx(&packetHeader, &packetData);

    if (result != PacketCommunicatorReceiveResult::Ok)
    {
        packet = nullptr;
        return result;
    }

    packet = CreatePacket(*packetHeader, packetData, DataLink);
    return result;
}

PacketCommunicatorReceiveResult PacketCommunicator::ReceiveSomePackets([Out] int% countGot, int maxPackets, HandlePacket^ callBack)
{
    AssertMode(PacketCommunicatorMode::Capture);

    PacketHandler^ packetHandler = gcnew PacketHandler(callBack, DataLink);
    HandlerDelegate^ packetHandlerDelegate = gcnew HandlerDelegate(packetHandler, &PacketHandler::Handle);
    pcap_handler functionPointer = (pcap_handler)Marshal::GetFunctionPointerForDelegate(packetHandlerDelegate).ToPointer();

    countGot = pcap_dispatch(_pcapDescriptor, 
                             maxPackets, 
                             functionPointer,
                             NULL);
    GC::KeepAlive(packetHandlerDelegate);

    switch (countGot)
    {
    case -2:
        countGot = 0;
        return PacketCommunicatorReceiveResult::BreakLoop;
    case -1:
        throw BuildInvalidOperation("Failed reading from device");
    case 0:
        if (packetHandler->PacketCounter != 0)
        {
            countGot = packetHandler->PacketCounter;
            return PacketCommunicatorReceiveResult::Eof;
        }
    }

    return PacketCommunicatorReceiveResult::Ok;
}

PacketCommunicatorReceiveResult PacketCommunicator::ReceivePackets(int count, HandlePacket^ callback)
{
    AssertMode(PacketCommunicatorMode::Capture);

    PacketHandler^ packetHandler = gcnew PacketHandler(callback, DataLink);
    HandlerDelegate^ packetHandlerDelegate = gcnew HandlerDelegate(packetHandler, &PacketHandler::Handle);
    pcap_handler functionPointer = (pcap_handler)Marshal::GetFunctionPointerForDelegate(packetHandlerDelegate).ToPointer();

    int result = pcap_loop(_pcapDescriptor, count, functionPointer, NULL);
    GC::KeepAlive(packetHandlerDelegate);

    switch (result)
    {
    case -2:
        return PacketCommunicatorReceiveResult::BreakLoop;
    case -1:
        throw BuildInvalidOperation("Failed reading from device");
    case 0:
        if (packetHandler->PacketCounter != count)
            return PacketCommunicatorReceiveResult::Eof;
    }

    return PacketCommunicatorReceiveResult::Ok;
}

PacketCommunicatorReceiveResult PacketCommunicator::ReceiveStatistics([Out] PacketSampleStatistics^% statistics)
{
    AssertMode(PacketCommunicatorMode::Statistics);

    pcap_pkthdr* packetHeader;
    const unsigned char* packetData;
    PacketCommunicatorReceiveResult result = RunPcapNextEx(&packetHeader, &packetData);

    if (result != PacketCommunicatorReceiveResult::Ok)
        throw gcnew InvalidOperationException("Got result " + result.ToString() + " in statistics mode");

    statistics = gcnew PacketSampleStatistics(*packetHeader, packetData);

    return result;
}

PacketCommunicatorReceiveResult PacketCommunicator::ReceiveStatistics(int count, HandleStatistics^ callBack)
{
    AssertMode(PacketCommunicatorMode::Statistics);

    StatisticsHandler^ statisticsHandler = gcnew StatisticsHandler(callBack);
    HandlerDelegate^ statisticsHandlerDelegate = gcnew HandlerDelegate(statisticsHandler, &StatisticsHandler::Handle);
    pcap_handler functionPointer = (pcap_handler)Marshal::GetFunctionPointerForDelegate(statisticsHandlerDelegate).ToPointer();

    int result = pcap_loop(_pcapDescriptor, count, functionPointer, NULL);
    GC::KeepAlive(statisticsHandlerDelegate);

    if (result == -1)
        throw BuildInvalidOperation("Failed reading from device");
    if (result == -2)
        return PacketCommunicatorReceiveResult::BreakLoop;
    return PacketCommunicatorReceiveResult::Ok;
}

void PacketCommunicator::Break()
{
    pcap_breakloop(_pcapDescriptor);
}

void PacketCommunicator::SendPacket(Packet^ packet)
{
    if (packet->Length == 0)
        return;
    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    if (pcap_sendpacket(_pcapDescriptor, unamangedPacketBytes, packet->Length) != 0)
        throw BuildInvalidOperation("Failed writing to device");
}

BerkeleyPacketFilter^ PacketCommunicator::CreateFilter(String^ filterValue)
{
    return gcnew BerkeleyPacketFilter(_pcapDescriptor, filterValue, _ipV4Netmask);
}

void PacketCommunicator::SetFilter(BerkeleyPacketFilter^ filter)
{
    filter->SetFilter(_pcapDescriptor);
}

void PacketCommunicator::SetFilter(String^ filterValue)
{
    BerkeleyPacketFilter^ filter = CreateFilter(filterValue);
    try
    {
        SetFilter(filter);
    }
    finally
    {
        filter->~BerkeleyPacketFilter();
    }
}

PacketDumpFile^ PacketCommunicator::OpenDump(System::String^ fileName)
{
    return gcnew PacketDumpFile(_pcapDescriptor, fileName);
}

PacketCommunicator::~PacketCommunicator()
{
    pcap_close(_pcapDescriptor);
}

// Internal

PacketCommunicator::PacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth *auth, SocketAddress^ netmask)
{
    // Open the device
    char errorBuffer[PCAP_ERRBUF_SIZE];
    pcap_t *pcapDescriptor = pcap_open(source,                // name of the device
                                       snapshotLength,        // portion of the packet to capture
                                                              // 65536 guarantees that the whole packet will be captured on all the link layers
                                       safe_cast<int>(attributes),
                                       readTimeout,           // read timeout
                                       auth,                  // authentication on the remote machine
                                       errorBuffer);               // error buffer

    if (pcapDescriptor == NULL)
		throw gcnew InvalidOperationException(String::Format(CultureInfo::InvariantCulture, "Unable to open the adapter. Adapter name: {0}. WinPcap Error: {1}", gcnew String(source), gcnew String(errorBuffer)));

    _pcapDescriptor = pcapDescriptor;
    _ipV4Netmask = dynamic_cast<IpV4SocketAddress^>(netmask);
}

// Protected

pcap_t* PacketCommunicator::PcapDescriptor::get()
{
    return _pcapDescriptor;
}

InvalidOperationException^ PacketCommunicator::BuildInvalidOperation(System::String^ errorMessage)
{
    return PcapError::BuildInvalidOperation(errorMessage, _pcapDescriptor);
}

// Private

// static
Packet^ PacketCommunicator::CreatePacket(const pcap_pkthdr& packetHeader, const unsigned char* packetData, IDataLink^ dataLink)
{
    DateTime timestamp;
    PacketTimestamp::PcapTimestampToDateTime(packetHeader.ts, timestamp);

    array<Byte>^ managedPacketData = MarshalingServices::UnamangedToManagedByteArray(packetData, 0, packetHeader.caplen);
    return gcnew Packet(managedPacketData, timestamp, dataLink);
}

PacketCommunicatorReceiveResult PacketCommunicator::RunPcapNextEx(pcap_pkthdr** packetHeader, const unsigned char** packetData)
{
    int result = pcap_next_ex(_pcapDescriptor, packetHeader, packetData);
    switch (result)
    {
    case -2: 
        return PacketCommunicatorReceiveResult::Eof;
    case -1: 
        throw PcapError::BuildInvalidOperation("Failed reading from device", _pcapDescriptor);
    case 0: 
        return PacketCommunicatorReceiveResult::Timeout;
    case 1: 
        return PacketCommunicatorReceiveResult::Ok;
    default: 
        throw gcnew InvalidOperationException("Result value " + result.ToString(CultureInfo::InvariantCulture) + " is undefined");
    }
}

void PacketCommunicator::AssertMode(PacketCommunicatorMode mode)
{
    if (Mode != mode)
        throw gcnew InvalidOperationException("Wrong Mode. Must be in mode " + mode.ToString() + " and not in mode " + Mode.ToString());
}

void PacketCommunicator::PacketHandler::Handle(unsigned char *, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData)
{
    ++_packetCounter;
    _callback->Invoke(CreatePacket(*packetHeader, packetData, _dataLink));
}

int PacketCommunicator::PacketHandler::PacketCounter::get()
{
    return _packetCounter;
}

void PacketCommunicator::StatisticsHandler::Handle(unsigned char *, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData)
{
    _callback->Invoke(gcnew PacketSampleStatistics(*packetHeader, packetData));
}