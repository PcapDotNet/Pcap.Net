#include "PcapLibrary.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

// static
String^ PcapLibrary::Version::get()
{
    return gcnew String(pcap_lib_version());
}