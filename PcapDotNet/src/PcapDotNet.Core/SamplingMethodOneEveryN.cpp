#include "SamplingMethodOneEveryN.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

SamplingMethodOneEveryCount::SamplingMethodOneEveryCount(int count)
{
    if (count <= 0)
        throw gcnew ArgumentOutOfRangeException("count", count, "Must be positive");
    _count = count;
}

int SamplingMethodOneEveryCount::Method::get()
{
    return PCAP_SAMP_1_EVERY_N;
}

int SamplingMethodOneEveryCount::Value::get()
{
    return _count;
}
