#include "PcapLiveDevice.h"

#include <string>

#include "MarshalingServices.h"
#include "Pcap.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace BPacket;
using namespace PcapDotNet::Core;

ReadOnlyCollection<PcapLiveDevice^>^ PcapLiveDevice::AllLocalMachine::get()
{
    pcap_if_t *alldevs;
    char errbuf[PCAP_ERRBUF_SIZE];

    // Retrieve the device list from the local machine 
    if (pcap_findalldevs_ex(PCAP_SRC_IF_STRING, NULL, // auth is not needed 
        &alldevs, errbuf) == -1)
    {
        String^ errorString = gcnew String(errbuf);
        throw gcnew InvalidOperationException(String::Format("Failed getting devices. Error: %s", errorString));
    }
    
    try
    {
        List<PcapLiveDevice^>^ result = gcnew List<PcapLiveDevice^>();
        for (pcap_if_t *d = alldevs; d != NULL; d = d->next)
        {
            // IP addresses
            List<PcapAddress^>^ addresses = gcnew List<PcapAddress^>();
            for (pcap_addr_t *a = d->addresses; a; a = a->next) 
            {
                PcapAddress^ deviceAddress = gcnew PcapAddress(a);
                addresses->Add(deviceAddress);
            }

            result->Add(gcnew PcapLiveDevice(gcnew String(d->name), 
                                     gcnew String(d->description), 
                                     safe_cast<DeviceFlags>(d->flags),
                                     gcnew ReadOnlyCollection<PcapAddress^>(addresses)));
        }
        return gcnew ReadOnlyCollection<PcapLiveDevice^>(result);
    }
    finally
    {
        // We don't need any more the device list. Free it 
        pcap_freealldevs(alldevs);
    }
}

String^ PcapLiveDevice::Name::get()
{ 
    return _name;
}

String^ PcapLiveDevice::Description::get()
{ 
    return _description; 
}

DeviceFlags^ PcapLiveDevice::Flags::get()
{
    return _flags;
}

ReadOnlyCollection<PcapAddress^>^ PcapLiveDevice::Addresses::get()
{
    return gcnew ReadOnlyCollection<PcapAddress^>(_addresses);
}

PcapDeviceHandler^ PcapLiveDevice::Open(int snapshotLength, PcapDeviceOpenFlags flags, int readTimeout)
{
    std::string deviceName = MarshalingServices::ManagedToUnmanagedString(Name);

    // Get the netmask
    SocketAddress^ netmask;
    if (Addresses->Count != 0)
        netmask = Addresses[0]->Netmask;

    // Open the device
    return gcnew PcapDeviceHandler(deviceName.c_str(), snapshotLength, flags, readTimeout, NULL, netmask);
}

// Private Methods

PcapLiveDevice::PcapLiveDevice(String^ name, String^ description, DeviceFlags^ flags, ReadOnlyCollection<PcapAddress^>^ addresses)
{
    _name = name;
    _description = description;
    _flags = flags;
    _addresses = addresses;
}
