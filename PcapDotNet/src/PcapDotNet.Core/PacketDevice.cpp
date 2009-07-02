#include "PacketDevice.h"

using namespace PcapDotNet::Core;

PacketCommunicator^ PacketDevice::Open()
{
    return Open(DefaultSnapshotLength, PacketDeviceOpenFlags::Promiscuous, 1000);
}
