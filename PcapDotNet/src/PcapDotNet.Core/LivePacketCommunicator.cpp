#include "LivePacketCommunicator.h"

#include "Pcap.h"

using namespace PcapDotNet::Core;

LivePacketCommunicator::LivePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth* auth, SocketAddress^ netmask)
: PacketCommunicator(source, snapshotLength, attributes, readTimeout, auth, netmask)
{
}

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
    sendBuffer->Transmit(PcapDescriptor, isSync);
}
