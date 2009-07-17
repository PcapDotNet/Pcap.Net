#include "PacketDevice.h"

using namespace PcapDotNet::Core;

PacketCommunicator^ PacketDevice::Open()
{
    return Open(DefaultSnapshotLength, PacketDeviceOpenAttributes::Promiscuous, 1000);
}
