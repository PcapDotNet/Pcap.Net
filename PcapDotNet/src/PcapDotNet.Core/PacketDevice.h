#pragma once

#include "IPacketDevice.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PacketDevice abstract : IPacketDevice
    {
    public:
        static const int DefaultSnapshotLength = 65536;

        virtual property System::String^ Name
        {
            System::String^ get() = 0;
        }

        virtual property System::String^ Description
        {
            System::String^ get() = 0;
        }

        virtual property DeviceFlags^ Flags
        {
            DeviceFlags^ get() = 0;
        }

        virtual property System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ Addresses
        {
            System::Collections::ObjectModel::ReadOnlyCollection<DeviceAddress^>^ get() = 0;
        }

        virtual PacketCommunicator^ Open(int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout) = 0;

        virtual PacketCommunicator^ Open();

    protected:
        PacketDevice(){}
    };
}}