#include "PcapStatistics.h"

using namespace System;
using namespace PcapDotNet;

PcapStatistics::PcapStatistics(DateTime timestamp, unsigned long acceptedPackets, unsigned long acceptedBytes)
{
    _timestamp = timestamp;
    _acceptedPackets = acceptedPackets;
    _acceptedBytes = acceptedBytes;
}

DateTime PcapStatistics::Timestamp::get()
{
    return _timestamp;
}
unsigned long PcapStatistics::AcceptedPackets::get()
{
    return _acceptedPackets;
}

unsigned long PcapStatistics::AcceptedBytes::get()
{
    return _acceptedBytes;
}
        
System::String^ PcapStatistics::ToString()
{
    return _timestamp + ": " + AcceptedPackets + " packets. " + AcceptedBytes + " bytes.";
}