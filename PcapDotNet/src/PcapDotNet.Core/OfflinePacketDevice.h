#pragma once

#include "PacketDevice.h"

namespace PcapDotNet { namespace Core 
{
    public ref class OfflinePacketDevice : PacketDevice
    {
    public:
        OfflinePacketDevice(System::String^ fileName);

        virtual property System::String^ Name
        {
            System::String^ get() override;
        }

        virtual property System::String^ Description
        {
            System::String^ get() override;
        }

        virtual property DeviceFlags^ Flags
        {
            DeviceFlags^ get() override;
        }

        virtual property System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ Addresses
        {
            System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ get() override;
        }

        virtual PacketCommunicator^ Open(int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout) override;

    private:
        System::String^ _fileName;
    };
}}