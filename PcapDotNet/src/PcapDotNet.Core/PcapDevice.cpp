#include "PcapDevice.h"

using namespace PcapDotNet::Core;

PcapDeviceHandler^ PcapDevice::Open()
{
    return Open(DefaultSnapshotLength, PcapDeviceOpenFlags::Promiscuous, 1000);
}
