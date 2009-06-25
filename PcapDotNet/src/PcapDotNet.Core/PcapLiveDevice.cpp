#include "PcapLiveDevice.h"

#include <stdio.h>
#include <pcap.h>
#include <remote-ext.h>
#include <string>

#include "MarshalingServices.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace BPacket;
using namespace PcapDotNet;

List<PcapLiveDevice^>^ PcapLiveDevice::AllLocalMachine::get()
{
    pcap_if_t *alldevs;
    char errbuf[PCAP_ERRBUF_SIZE];

    // Retrieve the device list from the local machine 
    if (pcap_findalldevs_ex(PCAP_SRC_IF_STRING, NULL // auth is not needed 
        , &alldevs, errbuf) == -1)
    {
        String^ errorString = gcnew String(errbuf);
        throw gcnew InvalidOperationException(String::Format("Error in pcap_findalldevs_ex: %s\n", errorString));
    }
    
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
                                 addresses));
    }

    // We don't need any more the device list. Free it 
    pcap_freealldevs(alldevs);
    return result;
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

List<PcapAddress^>^ PcapLiveDevice::Addresses::get()
{
    return _addresses;
}

PcapDeviceHandler^ PcapLiveDevice::Open(int snapLen, PcapDeviceOpenFlags flags, int readTimeout)
{
    pcap_t *adhandle;
    char errbuf[PCAP_ERRBUF_SIZE];

    std::string deviceName = MarshalingServices::ManagedToUnmanagedString(Name);
    
    // Open the device
    adhandle = pcap_open(deviceName.c_str(),    // name of the device
                         snapLen,               // portion of the packet to capture
                                                // 65536 guarantees that the whole packet will be captured on all the link layers
                         safe_cast<int>(flags),
                         readTimeout,           // read timeout
                         NULL,                  // authentication on the remote machine
                         errbuf);               // error buffer

    if (adhandle == NULL)
    {
        gcnew InvalidOperationException(String::Format("Unable to open the adapter. %s is not supported by WinPcap", Name));
    }

    SocketAddress^ netmask;
    if (Addresses->Count != 0)
        netmask = Addresses[0]->Netmask;
    return gcnew PcapDeviceHandler(adhandle, netmask);
}

// Private Methods

PcapLiveDevice::PcapLiveDevice(String^ name, String^ description, DeviceFlags^ flags, List<PcapAddress^>^ addresses)
{
    _name = name;
    _description = description;
    _flags = flags;
    _addresses = addresses;
}
