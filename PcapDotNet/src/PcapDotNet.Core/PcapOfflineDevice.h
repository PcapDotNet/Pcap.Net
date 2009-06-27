#pragma once

#include "PcapDevice.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PcapOfflineDevice : PcapDevice
    {
    public:
        PcapOfflineDevice(System::String^ filename);

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

        virtual property System::Collections::ObjectModel::ReadOnlyCollection<PcapAddress^>^ Addresses
        {
            System::Collections::ObjectModel::ReadOnlyCollection<PcapAddress^>^ get() override;
        }

        virtual PcapDeviceHandler^ Open(int snapshotLength, PcapDeviceOpenFlags flags, int readTimeout) override;

    private:
        System::String^ _filename;
    };
}}