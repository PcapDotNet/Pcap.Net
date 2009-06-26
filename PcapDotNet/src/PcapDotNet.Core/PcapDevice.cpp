#include "PcapDevice.h"

using namespace PcapDotNet;

PcapDeviceHandler^ PcapDevice::Open()
{
    return Open(DefaultSnapLen, PcapDeviceOpenFlags::Promiscuous, 1000);
}
