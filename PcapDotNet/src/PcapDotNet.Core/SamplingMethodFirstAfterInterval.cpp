#include "SamplingMethodFirstAfterInterval.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

SamplingMethodFirstAfterInterval::SamplingMethodFirstAfterInterval(int intervalInMilliseconds)
{
    if (intervalInMilliseconds < 0)
        throw gcnew ArgumentOutOfRangeException("intervalInMilliseconds", intervalInMilliseconds, "Must be non negative");
    _intervalInMilliseconds = intervalInMilliseconds;
}

SamplingMethodFirstAfterInterval::SamplingMethodFirstAfterInterval(TimeSpan interval)
{
    double intervalInMilliseconds = interval.TotalMilliseconds;
    if (intervalInMilliseconds > Int32::MaxValue)
        throw gcnew ArgumentOutOfRangeException("interval", interval, "Must be smaller than " + TimeSpan::FromMilliseconds(Int32::MaxValue).ToString());
    if (intervalInMilliseconds < 0)
        throw gcnew ArgumentOutOfRangeException("interval", interval, "Must be non negative");

    _intervalInMilliseconds = (int)intervalInMilliseconds;
}
    
int SamplingMethodFirstAfterInterval::Method::get()
{
    return PCAP_SAMP_FIRST_AFTER_N_MS;
}

int SamplingMethodFirstAfterInterval::Value::get()
{
    return _intervalInMilliseconds;
}
