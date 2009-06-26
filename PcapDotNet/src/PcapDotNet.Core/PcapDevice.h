#pragma once

#include "IPcapDevice.h"

namespace PcapDotNet 
{
    public ref class PcapDevice abstract : IPcapDevice
    {
    public:
        static const int DefaultSnapLen = 65536;

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

        virtual property System::Collections::Generic::List<PcapAddress^>^ Addresses
        {
            System::Collections::Generic::List<PcapAddress^>^ get() = 0;
        }

        virtual PcapDeviceHandler^ Open(int snapLen, PcapDeviceOpenFlags flags, int readTimeout) = 0;

        virtual PcapDeviceHandler^ Open();
    };
}