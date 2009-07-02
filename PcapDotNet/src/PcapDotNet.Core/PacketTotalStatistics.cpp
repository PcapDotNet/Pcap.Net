#include "PacketTotalStatistics.h"

using namespace PcapDotNet::Core;

PacketTotalStatistics::PacketTotalStatistics(unsigned int packetsReceived, unsigned int packetsDroppedByDriver, unsigned int packetsDroppedByInterface, unsigned int packetsCaptured)
{
    _packetsReceived = packetsReceived;
    _packetsDroppedByDriver = packetsDroppedByDriver;
    _packetsDroppedByInterface = packetsDroppedByInterface;
    _packetsCaptured = packetsCaptured;
}

unsigned int PacketTotalStatistics::PacketsReceived::get()
{
    return _packetsReceived;
}

unsigned int PacketTotalStatistics::PacketsDroppedByDriver::get()
{
    return _packetsDroppedByDriver;
}

unsigned int PacketTotalStatistics::PacketsDroppedByInterface::get()
{
    return _packetsDroppedByInterface;
}

unsigned int PacketTotalStatistics::PacketsCaptured::get()
{
    return _packetsCaptured;
}

bool PacketTotalStatistics::Equals(PacketTotalStatistics^ other)
{
    if (other == nullptr)
        return false;

    return (PacketsReceived == other->PacketsReceived &&
            PacketsDroppedByDriver == other->PacketsDroppedByDriver &&
            PacketsDroppedByInterface == other->PacketsDroppedByInterface &&
            PacketsCaptured == other->PacketsCaptured);
}

bool PacketTotalStatistics::Equals(Object^ other)
{
    return Equals(dynamic_cast<PacketTotalStatistics^>(other));
}
