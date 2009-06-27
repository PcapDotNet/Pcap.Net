#include "PcapSampleStatistics.h"

using namespace System;
using namespace PcapDotNet::Core;

PcapSampleStatistics::PcapSampleStatistics(DateTime timestamp, unsigned long acceptedPackets, unsigned long acceptedBytes)
{
    _timestamp = timestamp;
    _acceptedPackets = acceptedPackets;
    _acceptedBytes = acceptedBytes;
}

DateTime PcapSampleStatistics::Timestamp::get()
{
    return _timestamp;
}
unsigned long PcapSampleStatistics::AcceptedPackets::get()
{
    return _acceptedPackets;
}

unsigned long PcapSampleStatistics::AcceptedBytes::get()
{
    return _acceptedBytes;
}
        
System::String^ PcapSampleStatistics::ToString()
{
    return _timestamp + ": " + AcceptedPackets + " packets. " + AcceptedBytes + " bytes.";
}