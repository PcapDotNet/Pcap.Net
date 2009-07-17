#pragma once

#include "PacketDevice.h"

namespace PcapDotNet { namespace Core 
{
    public ref class LivePacketDevice : PacketDevice
    {
    public:
        static property System::Collections::ObjectModel::ReadOnlyCollection<LivePacketDevice^>^ AllLocalMachine
        {
            System::Collections::ObjectModel::ReadOnlyCollection<LivePacketDevice^>^ get();
        }

        virtual property System::String^ Name
        {
            System::String^ get() override;
        }

        virtual property System::String^ Description
        {
            System::String^ get() override;
        }

        virtual property DeviceAttributes^ Attributes
        {
            DeviceAttributes^ get() override;
        }

        virtual property System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ Addresses
        {
            System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ get() override;
        }

        /// <summary>
        /// Open a generic source in order to capture / send (WinPcap only) traffic. 
        /// </summary>
        /// <param name="snapshotLength">length of the packet that has to be retained. For each packet received by the filter, only the first 'snapshotLength' bytes are stored in the buffer and passed to the user application. For instance, snaplen equal to 100 means that only the first 100 bytes of each packet are stored.</param>
        /// <param name="attributes">keeps several flags that can be needed for capturing packets.</param>
        /// <param name="readTimeout">read timeout in milliseconds. The read timeout is used to arrange that the read not necessarily return immediately when a packet is seen, but that it waits for some amount of time to allow more packets to arrive and to read multiple packets from the OS kernel in one operation. Not all platforms support a read timeout; on platforms that don't, the read timeout is ignored.</param>
        virtual PacketCommunicator^ Open(int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout) override;

     private:
        LivePacketDevice(System::String^ name, System::String^ description, DeviceAttributes^ attributes, System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ addresses);

    private:
        System::String^ _name;
        System::String^ _description;
        DeviceAttributes^ _attributes;
        System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ _addresses;
    };
}}