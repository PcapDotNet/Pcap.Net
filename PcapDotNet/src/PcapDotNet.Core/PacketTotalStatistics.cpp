#include "PacketTotalStatistics.h"

using namespace System;
using namespace System::Text;
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

bool PacketTotalStatistics::Equals(Object^ obj)
{
    return Equals(dynamic_cast<PacketTotalStatistics^>(obj));
}

int PacketTotalStatistics::GetHashCode()
{
    return 
        _packetsReceived ^ 
        _packetsDroppedByDriver ^ 
        _packetsDroppedByInterface ^ 
        _packetsCaptured;
}

String^ PacketTotalStatistics::ToString()
{
    StringBuilder^ stringBuilder = gcnew StringBuilder();
    stringBuilder->Append("Packets Received: ");
    stringBuilder->Append(PacketsReceived);
    stringBuilder->Append(". ");
    stringBuilder->Append("Packets Dropped By Driver: ");
    stringBuilder->Append(PacketsDroppedByDriver);
    stringBuilder->Append(". ");
    stringBuilder->Append("Packets Dropped By Interface: ");
    stringBuilder->Append(PacketsDroppedByInterface);
    stringBuilder->Append(". ");
    stringBuilder->Append("Packets Captured: ");
    stringBuilder->Append(PacketsCaptured);
    stringBuilder->Append(".");
    return stringBuilder->ToString();
}