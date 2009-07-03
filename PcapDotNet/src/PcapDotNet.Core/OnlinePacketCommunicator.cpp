#include "OnlinePacketCommunicator.h"

#include "Pcap.h"

using namespace PcapDotNet::Core;

OnlinePacketCommunicator::OnlinePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout, pcap_rmtauth* auth, SocketAddress^ netmask)
: PacketCommunicator(source, snapshotLength, flags, readTimeout, auth, netmask)
{
}

PacketTotalStatistics^ OnlinePacketCommunicator::TotalStatistics::get()
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
    return gcnew PacketTotalStatistics(packetsReceived, packetsDroppedByDriver, packetsDroppedByInterface, packetsCaptured);
}
