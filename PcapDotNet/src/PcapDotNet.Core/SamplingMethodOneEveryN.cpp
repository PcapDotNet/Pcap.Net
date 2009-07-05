#include "SamplingMethodOneEveryN.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

SamplingMethodOneEveryN::SamplingMethodOneEveryN(int n)
{
    if (n <= 0)
        throw gcnew ArgumentOutOfRangeException("n", n, "Must be positive");
    _n = n;
}

int SamplingMethodOneEveryN::Method::get()
{
    return PCAP_SAMP_1_EVERY_N;
}

int SamplingMethodOneEveryN::Value::get()
{
    return _n;
}
