#include "PcapOfflineDevice.h"

#include <string>

#include "Pcap.h"
#include "MarshalingServices.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace PcapDotNet;

PcapOfflineDevice::PcapOfflineDevice(System::String^ filename)
{
    _filename = filename;
}

String^ PcapOfflineDevice::Name::get()
{
    return _filename;
}

String^ PcapOfflineDevice::Description::get()
{
    return String::Empty;
}

DeviceFlags^ PcapOfflineDevice::Flags::get()
{
    return DeviceFlags::None;
}

ReadOnlyCollection<PcapAddress^>^ PcapOfflineDevice::Addresses::get()
{
    return gcnew ReadOnlyCollection<PcapAddress^>(gcnew List<PcapAddress^>());
}

PcapDeviceHandler^ PcapOfflineDevice::Open(int snapshotLength, PcapDeviceOpenFlags flags, int readTimeout)
{
    std::string unamangedFilename = MarshalingServices::ManagedToUnmanagedString(_filename);

    // Create the source string according to the new WinPcap syntax
    char source[PCAP_BUF_SIZE];
    char errbuf[PCAP_ERRBUF_SIZE];
    if (pcap_createsrcstr(source,         // variable that will keep the source string
                          PCAP_SRC_FILE,  // we want to open a file
                          NULL,           // remote host
                          NULL,           // port on the remote host
                          unamangedFilename.c_str(),        // name of the file we want to open
                          errbuf          // error buffer
                          ) != 0)
    {
        throw gcnew InvalidOperationException("Error creating a source string from filename " + _filename + " Error: " + gcnew String(errbuf));
    }

    return gcnew PcapDeviceHandler(source, snapshotLength, flags, readTimeout, NULL, nullptr);
}
