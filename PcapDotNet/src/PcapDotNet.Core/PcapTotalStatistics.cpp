#include "PcapTotalStatistics.h"

using namespace PcapDotNet::Core;

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

bool PcapTotalStatistics::Equals(PcapTotalStatistics^ other)
{
    if (other == nullptr)
        return false;

    return (PacketsReceived == other->PacketsReceived &&
            PacketsDroppedByDriver == other->PacketsDroppedByDriver &&
            PacketsDroppedByInterface == other->PacketsDroppedByInterface &&
            PacketsCaptured == other->PacketsCaptured);
}

bool PcapTotalStatistics::Equals(Object^ other)
{
    return Equals(dynamic_cast<PcapTotalStatistics^>(other));
}
