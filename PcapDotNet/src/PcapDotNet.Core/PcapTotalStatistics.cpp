#include "PcapTotalStatistics.h"

using namespace PcapDotNet;

PcapTotalStatistics::PcapTotalStatistics(unsigned int packetsReceived, unsigned int packetsDroppedByDriver, unsigned int packetsDroppedByInterface, unsigned int packetsCaptured)
{
    _packetsReceived = packetsReceived;
    _packetsDroppedByDriver = packetsDroppedByDriver;
    _packetsDroppedByInterface = packetsDroppedByInterface;
    _packetsCaptured = packetsCaptured;
}

unsigned int PcapTotalStatistics::PacketsReceived::get()
{
    return _packetsReceived;
}

unsigned int PcapTotalStatistics::PacketsDroppedByDriver::get()
{
    return _packetsDroppedByDriver;
}

unsigned int PcapTotalStatistics::PacketsDroppedByInterface::get()
{
    return _packetsDroppedByInterface;
}

unsigned int PcapTotalStatistics::PacketsCaptured::get()
{
    return _packetsCaptured;
}
