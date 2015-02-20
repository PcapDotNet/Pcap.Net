#include "PacketSampleStatistics.h"
#include "PacketTimestamp.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

DateTime PacketSampleStatistics::Timestamp::get()
{
    return _timestamp;
}
unsigned __int64 PacketSampleStatistics::AcceptedPackets::get()
{
    return _acceptedPackets;
}

unsigned __int64 PacketSampleStatistics::AcceptedBytes::get()
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
    PacketTimestamp::PcapTimestampToDateTime(packetHeader.ts, _timestamp);

    _acceptedPackets = *reinterpret_cast<const unsigned __int64*>(packetData);
    _acceptedBytes = *reinterpret_cast<const unsigned __int64*>(packetData + 8);
}