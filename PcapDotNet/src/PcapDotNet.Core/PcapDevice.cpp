#include "PcapDevice.h"

using namespace PcapDotNet;

PcapDeviceHandler^ PcapDevice::Open()
{
    return Open(DefaultSnapshotLength, PcapDeviceOpenFlags::Promiscuous, 1000);
}
