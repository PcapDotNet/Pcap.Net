#include "PacketTimestamp.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Base;
using namespace PcapDotNet::Core;

// static 
DateTime PacketTimestamp::MinimumPacketTimestamp::get()
{
    return _minimumPacketTimestamp;
}

// static 
DateTime PacketTimestamp::MaximumPacketTimestamp::get()
{
    return _maximumPacketTimestamp;
}

// static
void PacketTimestamp::PcapTimestampToDateTime(const timeval& pcapTimestamp, [Runtime::InteropServices::Out] DateTime% dateTime)
{
    dateTime = DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind::Utc);
    TimeSpan seconds = TimeSpan::FromSeconds(pcapTimestamp.tv_sec);
    TimeSpan microseconds = TimeSpan::FromTicks(pcapTimestamp.tv_usec * TimeSpanExtensions::TicksPerMicrosecond);
    dateTime = dateTime.Add(seconds + microseconds);
    dateTime = dateTime.ToLocalTime();
}

// static 
void PacketTimestamp::DateTimeToPcapTimestamp(DateTime dateTime, timeval& pcapTimestamp)
{
    dateTime = dateTime.ToUniversalTime();
    TimeSpan timespan = dateTime - DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind::Utc);
    pcapTimestamp.tv_sec = (long)timespan.TotalSeconds;
    pcapTimestamp.tv_usec = (long)((timespan.TotalMilliseconds - 1000 * (double)pcapTimestamp.tv_sec) * 1000);
}

// static
void PacketTimestamp::Initialize()
{
    // Min
    timeval pcapTimestamp;
    pcapTimestamp.tv_sec = Int32::MinValue;
    pcapTimestamp.tv_usec = Int32::MinValue;
    PacketTimestamp::PcapTimestampToDateTime(pcapTimestamp, _minimumPacketTimestamp);

    pcapTimestamp.tv_sec = Int32::MaxValue;
    pcapTimestamp.tv_usec = Int32::MaxValue;
    PacketTimestamp::PcapTimestampToDateTime(pcapTimestamp, _maximumPacketTimestamp);
}