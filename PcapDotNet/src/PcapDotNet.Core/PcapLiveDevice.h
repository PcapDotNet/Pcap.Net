#pragma once

#include "PcapDevice.h"

namespace PcapDotNet 
{
    public ref class PcapLiveDevice : PcapDevice
    {
    public:
        static property System::Collections::Generic::List<PcapLiveDevice^>^ AllLocalMachine
        {
            System::Collections::Generic::List<PcapLiveDevice^>^ get();
        }

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

        virtual property System::Collections::Generic::List<PcapAddress^>^ Addresses
        {
            System::Collections::Generic::List<PcapAddress^>^ get() override;
        }

        /// <summary>
        /// Open a generic source in order to capture / send (WinPcap only) traffic. 
        /// </summary>
        /// <param name="snapLen">length of the packet that has to be retained. For each packet received by the filter, only the first 'snaplen' bytes are stored in the buffer and passed to the user application. For instance, snaplen equal to 100 means that only the first 100 bytes of each packet are stored.</param>
        /// <param name="flags">keeps several flags that can be needed for capturing packets.</param>
        /// <param name="flags">read timeout in milliseconds. The read timeout is used to arrange that the read not necessarily return immediately when a packet is seen, but that it waits for some amount of time to allow more packets to arrive and to read multiple packets from the OS kernel in one operation. Not all platforms support a read timeout; on platforms that don't, the read timeout is ignored.</param>
        virtual PcapDeviceHandler^ Open(int snapLen, PcapDeviceOpenFlags flags, int readTimeout) override;

     private:
        PcapLiveDevice(System::String^ name, System::String^ description, DeviceFlags^ flags, System::Collections::Generic::List<PcapAddress^>^ addresses);

    private:
        System::String^ _name;
        System::String^ _description;
        DeviceFlags^ _flags;
        System::Collections::Generic::List<PcapAddress^>^ _addresses;
    };
}