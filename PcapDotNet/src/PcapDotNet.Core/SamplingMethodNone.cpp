#include "SamplingMethodNone.h"
#include "Pcap.h"

using namespace PcapDotNet::Core;

int SamplingMethodNone::Method::get()
{
    return PCAP_SAMP_NOSAMP;
}

int SamplingMethodNone::Value::get()
{
    return 0;
}
