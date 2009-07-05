#include "SamplingMethodFirstAfterInterval.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

SamplingMethodFirstAfterInterval::SamplingMethodFirstAfterInterval(int intervalInMs)
{
    if (intervalInMs < 0)
        throw gcnew ArgumentOutOfRangeException("intervalInMs", intervalInMs, "Must be non negative");
    _intervalInMs = intervalInMs;
}

SamplingMethodFirstAfterInterval::SamplingMethodFirstAfterInterval(TimeSpan interval)
{
    double intervalInMs = interval.TotalMilliseconds;
    if (intervalInMs > Int32::MaxValue)
        throw gcnew ArgumentOutOfRangeException("interval", interval, "Must be smaller than " + TimeSpan::FromMilliseconds(Int32::MaxValue).ToString());
    if (intervalInMs < 0)
        throw gcnew ArgumentOutOfRangeException("interval", interval, "Must be non negative");

    _intervalInMs = (int)intervalInMs;
}
    
int SamplingMethodFirstAfterInterval::Method::get()
{
    return PCAP_SAMP_FIRST_AFTER_N_MS;
}

int SamplingMethodFirstAfterInterval::Value::get()
{
    return _intervalInMs;
}
