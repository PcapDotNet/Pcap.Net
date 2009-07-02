#include "PacketSampleStatistics.h"

using namespace System;
using namespace PcapDotNet::Core;

PacketSampleStatistics::PacketSampleStatistics(DateTime timestamp, unsigned long acceptedPackets, unsigned long acceptedBytes)
{
    _timestamp = timestamp;
    _acceptedPackets = acceptedPackets;
    _acceptedBytes = acceptedBytes;
}

DateTime PacketSampleStatistics::Timestamp::get()
{
    return _timestamp;
}
unsigned long PacketSampleStatistics::AcceptedPackets::get()
{
    return _acceptedPackets;
}

unsigned long PacketSampleStatistics::AcceptedBytes::get()
{
    return _acceptedBytes;
}
        
System::String^ PacketSampleStatistics::ToString()
{
    return _timestamp + ": " + AcceptedPackets + " packets. " + AcceptedBytes + " bytes.";
}