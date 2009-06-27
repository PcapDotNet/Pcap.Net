#include "Timestamp.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

// static
void Timestamp::PcapTimestampToDateTime(const timeval& pcapTimestamp, [System::Runtime::InteropServices::Out] System::DateTime% dateTime)
{
    dateTime = DateTime(1970,1,1).Add(TimeSpan::FromSeconds(pcapTimestamp.tv_sec) + 
                                      TimeSpan::FromMilliseconds(((double)pcapTimestamp.tv_usec) / 1000));
}

// static 
void Timestamp::DateTimeToPcapTimestamp(System::DateTime dateTime, timeval& pcapTimestamp)
{
    TimeSpan timespan = dateTime - DateTime(1970,1,1);
    pcapTimestamp.tv_sec = (long)timespan.TotalSeconds;
    pcapTimestamp.tv_usec = (long)(timespan.Milliseconds * 1000);
}