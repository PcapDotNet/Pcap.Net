#include "PacketSampleStatistics.h"
#include "Timestamp.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

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

// Internal

PacketSampleStatistics::PacketSampleStatistics(const pcap_pkthdr& packetHeader, const unsigned char* packetData)
{
    PcapDotNet::Core::Timestamp::PcapTimestampToDateTime(packetHeader.ts, _timestamp);

    _acceptedPackets = *reinterpret_cast<const unsigned long*>(packetData);
    _acceptedBytes = *reinterpret_cast<const unsigned long*>(packetData + 8);
}