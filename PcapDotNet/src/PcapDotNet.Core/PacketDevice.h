#pragma once

#include "IPacketDevice.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// The base class of a live or offline interface.
    /// </summary>
    public ref class PacketDevice abstract : IPacketDevice
    {
    public:
        /// <summary>
        /// This snapshort length value should be sufficient, on most if not all networks, to capture all the data available from the packet.
        /// </summary>
        literal int DefaultSnapshotLength = 65536;

        /// <summary>
        /// A string giving a name for the device.
        /// </summary>
        virtual property System::String^ Name
        {
            System::String^ get() = 0;
        }

        /// <summary>
        /// if not null, a string giving a human-readable description of the device.
        /// </summary>
        virtual property System::String^ Description
        {
            System::String^ get() = 0;
        }

        /// <summary>
        /// Interface flags. Currently the only possible flag is Loopback, that is set if the interface is a loopback interface. 
        /// </summary>
        virtual property DeviceAttributes Attributes
        {
            DeviceAttributes get() = 0;
        }

        /// <summary>
        /// List of addresses for the interface.
        /// </summary>
        virtual property System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ Addresses
        {
            System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ get() = 0;
        }

        /// <summary>
        /// Open a generic source in order to capture / send (WinPcap only) traffic. 
        /// </summary>
        /// <param name="snapshotLength">Length of the packet that has to be retained. For each packet received by the filter, only the first 'snapshotLength' bytes are stored in the buffer and passed to the user application. For instance, snaplen equal to 100 means that only the first 100 bytes of each packet are stored.</param>
        /// <param name="attributes">Keeps several flags that can be needed for capturing packets.</param>
        /// <param name="readTimeout">Read timeout in milliseconds. The read timeout is used to arrange that the read not necessarily return immediately when a packet is seen, but that it waits for some amount of time to allow more packets to arrive and to read multiple packets from the OS kernel in one operation. Not all platforms support a read timeout; on platforms that don't, the read timeout is ignored.</param>
        /// <exception cref="System::InvalidOperationException">Thrown on failure.</exception>
        virtual PacketCommunicator^ Open(int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout) = 0;

        /// <summary>
        /// Open a generic source in order to capture / send (WinPcap only) traffic. 
        /// Uses maxmimum snapshotLength (65536), promiscuous mode and 1 second read timeout.
        /// </summary>
        virtual PacketCommunicator^ Open();

    protected:
        PacketDevice(){}
    };
}}