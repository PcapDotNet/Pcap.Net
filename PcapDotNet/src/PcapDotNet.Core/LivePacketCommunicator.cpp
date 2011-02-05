#include "LivePacketCommunicator.h"

#include "Pcap.h"

using namespace System;
using namespace System::Globalization;
using namespace PcapDotNet::Core;

PacketTotalStatistics^ LivePacketCommunicator::TotalStatistics::get()
{
    int statisticsSize;
    pcap_stat* statistics = pcap_stats_ex(PcapDescriptor, &statisticsSize);
    if (statistics == NULL)
        throw BuildInvalidOperation("Failed getting total statistics");

    return gcnew PacketTotalStatistics(*statistics, statisticsSize);
}

void LivePacketCommunicator::Transmit(PacketSendBuffer^ sendBuffer, bool isSync)
{
	if (sendBuffer == nullptr) 
		throw gcnew ArgumentNullException("sendBuffer");

	sendBuffer->Transmit(PcapDescriptor, isSync);
}

// Internal

LivePacketCommunicator::LivePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth* auth, SocketAddress^ netmask)
: PacketCommunicator(PcapOpen(source, snapshotLength, attributes, readTimeout, auth), netmask)
{
}

// Private

// static
pcap_t* LivePacketCommunicator::PcapOpen(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth *auth)
{
    // Open the device
    char errorBuffer[PCAP_ERRBUF_SIZE];
    pcap_t *pcapDescriptor = pcap_open(source,                // name of the device
                                       snapshotLength,        // portion of the packet to capture
                                                              // 65536 guarantees that the whole packet will be captured on all the link layers
                                       safe_cast<int>(attributes),
                                       readTimeout,           // read timeout
                                       auth,                  // authentication on the remote machine
                                       errorBuffer);          // error buffer

    if (pcapDescriptor == NULL)
        throw gcnew InvalidOperationException(String::Format(CultureInfo::InvariantCulture, "Unable to open the adapter. Adapter name: {0}. Error: {1}", gcnew String(source), gcnew String(errorBuffer)));

    return pcapDescriptor;
}
