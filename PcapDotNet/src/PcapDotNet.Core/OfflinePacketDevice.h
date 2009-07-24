#pragma once

#include "PacketDevice.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// An offline interface - a pcap file to read packets from.
    /// </summary>
    public ref class OfflinePacketDevice : PacketDevice
    {
    public:
        /// <summary>
        /// Creates a device object from a pcap file.
        /// The device can opened to read packets from.
        /// </summary>
        /// <param name="fileName">The name of the pcap file.</param>
        OfflinePacketDevice(System::String^ fileName);

        /// <summary>
        /// A string giving a name for the device.
        /// </summary>
        virtual property System::String^ Name
        {
            System::String^ get() override;
        }

        /// <summary>
        /// if not null, a string giving a human-readable description of the device.
        /// </summary>
        virtual property System::String^ Description
        {
            System::String^ get() override;
        }

        /// <summary>
        /// Interface flags. Currently the only possible flag is Loopback, that is set if the interface is a loopback interface. 
        /// </summary>
        virtual property DeviceAttributes Attributes
        {
            DeviceAttributes get() override;
        }

        /// <summary>
        /// List of addresses for the interface.
        /// </summary>
        virtual property System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ Addresses
        {
            System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ get() override;
        }

        /// <summary>
        /// Open a generic source in order to capture / send (WinPcap only) traffic. 
        /// </summary>
        /// <param name="snapshotLength">length of the packet that has to be retained. For each packet received by the filter, only the first 'snapshotLength' bytes are stored in the buffer and passed to the user application. For instance, snaplen equal to 100 means that only the first 100 bytes of each packet are stored.</param>
        /// <param name="attributes">Keeps several flags that can be needed for capturing packets.</param>
        /// <param name="readTimeout">Read timeout in milliseconds. The read timeout is used to arrange that the read not necessarily return immediately when a packet is seen, but that it waits for some amount of time to allow more packets to arrive and to read multiple packets from the OS kernel in one operation. Not all platforms support a read timeout; on platforms that don't, the read timeout is ignored.</param>
        /// <exception cref="System::InvalidOperationException">Thrown on failure.</exception>
        virtual PacketCommunicator^ Open(int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout) override;

    private:
        System::String^ _fileName;
    };
}}