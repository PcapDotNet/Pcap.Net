#include "OfflinePacketDevice.h"

#include <string>

#include "Pcap.h"
#include "OfflinePacketCommunicator.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace PcapDotNet::Core;

OfflinePacketDevice::OfflinePacketDevice(String^ fileName)
{
    _fileName = fileName;
}

String^ OfflinePacketDevice::Name::get()
{
    return _fileName;
}

String^ OfflinePacketDevice::Description::get()
{
    return String::Empty;
}

DeviceAttributes OfflinePacketDevice::Attributes::get()
{
    return DeviceAttributes::None;
}

ReadOnlyCollection<DeviceAddress^>^ OfflinePacketDevice::Addresses::get()
{
    return gcnew ReadOnlyCollection<DeviceAddress^>(gcnew List<DeviceAddress^>());
}

PacketCommunicator^ OfflinePacketDevice::Open(int /*snapshotLength*/, PacketDeviceOpenAttributes /*attributes*/, int /*readTimeout*/)
{
    return gcnew OfflinePacketCommunicator(_fileName);
}
