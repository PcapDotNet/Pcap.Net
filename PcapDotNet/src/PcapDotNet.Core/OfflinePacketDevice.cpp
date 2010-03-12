#include "OfflinePacketDevice.h"

#include <string>

#include "Pcap.h"
#include "MarshalingServices.h"
#include "OfflinePacketCommunicator.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::ObjectModel;
using namespace PcapDotNet::Core;

OfflinePacketDevice::OfflinePacketDevice(System::String^ fileName)
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

PacketCommunicator^ OfflinePacketDevice::Open(int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout)
{
    std::string unamangedFilename = MarshalingServices::ManagedToUnmanagedString(_fileName);

    // Create the source string according to the new WinPcap syntax
    char source[PCAP_BUF_SIZE];
    char errorBuffer[PCAP_ERRBUF_SIZE];
    if (pcap_createsrcstr(source,         // variable that will keep the source string
                          PCAP_SRC_FILE,  // we want to open a file
                          NULL,           // remote host
                          NULL,           // port on the remote host
                          unamangedFilename.c_str(),        // name of the file we want to open
                          errorBuffer          // error buffer
                          ) != 0)
    {
        throw gcnew InvalidOperationException("Error creating a source string from filename " + _fileName + " Error: " + gcnew String(errorBuffer));
    }

    return gcnew OfflinePacketCommunicator(source, snapshotLength, attributes, readTimeout, NULL);
}
