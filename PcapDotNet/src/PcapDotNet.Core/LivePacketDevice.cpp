#include "LivePacketDevice.h"

#include <string>

#include "MarshalingServices.h"
#include "Pcap.h"
#include "LivePacketCommunicator.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace System::Globalization;
using namespace PcapDotNet::Packets;
using namespace PcapDotNet::Core;

ReadOnlyCollection<LivePacketDevice^>^ LivePacketDevice::AllLocalMachine::get()
{
    pcap_if_t *alldevs;
    char errbuf[PCAP_ERRBUF_SIZE];

    // Retrieve the device list from the local machine 
    if (pcap_findalldevs_ex(PCAP_SRC_IF_STRING, NULL, // auth is not needed 
        &alldevs, errbuf) == -1)
    {
        String^ errorString = gcnew String(errbuf);
		throw gcnew InvalidOperationException(String::Format(CultureInfo::InvariantCulture, "Failed getting devices. Error: {0}", errorString));
    }
    
    try
    {
        List<LivePacketDevice^>^ result = gcnew List<LivePacketDevice^>();
        for (pcap_if_t *d = alldevs; d != NULL; d = d->next)
        {
            result->Add(gcnew LivePacketDevice(*d));
        }
        return gcnew ReadOnlyCollection<LivePacketDevice^>(result);
    }
    finally
    {
        // We don't need any more the device list. Free it 
        pcap_freealldevs(alldevs);
    }
}

String^ LivePacketDevice::Name::get()
{ 
    return _name;
}

String^ LivePacketDevice::Description::get()
{ 
    return _description; 
}

DeviceAttributes LivePacketDevice::Attributes::get()
{
    return _attributes;
}

ReadOnlyCollection<DeviceAddress^>^ LivePacketDevice::Addresses::get()
{
    return gcnew ReadOnlyCollection<DeviceAddress^>(_addresses);
}

PacketCommunicator^ LivePacketDevice::Open(int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout)
{
    std::string deviceName = MarshalingServices::ManagedToUnmanagedString(Name);

    // Get the netmask
    SocketAddress^ netmask;
    if (Addresses->Count != 0)
        netmask = Addresses[0]->Netmask;

    // Open the device
    return gcnew LivePacketCommunicator(deviceName.c_str(), snapshotLength, attributes, readTimeout, NULL, netmask);
}

// Private Methods

LivePacketDevice::LivePacketDevice(const pcap_if_t& device)
{
    // IP addresses
    List<DeviceAddress^>^ addresses = gcnew List<DeviceAddress^>();
    for (pcap_addr_t *a = device.addresses; a; a = a->next) 
    {
        DeviceAddress^ deviceAddress = gcnew DeviceAddress(a);
        addresses->Add(deviceAddress);
    }

    _name = gcnew String(device.name);
    _description = gcnew String(device.description);
    _attributes = safe_cast<DeviceAttributes>(device.flags);
    _addresses = gcnew ReadOnlyCollection<DeviceAddress^>(addresses);
}
