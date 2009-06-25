#pragma once

#include "PcapAddress.h"
#include "PcapDeviceHandler.h"

namespace PcapDotNet 
{
    [System::Flags]
    public enum class DeviceFlags : System::UInt32
    {
        None     = 0x00000000,
        LoopBack = 0x00000001
    };

    [System::Flags]
    public enum class PcapDeviceOpenFlags : System::Int32
    {
        None               = 0,
        PROMISCUOUS        = 1, // Defines if the adapter has to go in promiscuous mode.
        DATATX_UDP         = 2, // Defines if the data trasfer (in case of a remote capture) has to be done with UDP protocol.
        NOCAPTURE_RPCAP    = 4, // Defines if the remote probe will capture its own generated traffic.
        NOCAPTURE_LOCAL    = 8, // Defines if the local adapter will capture its own generated traffic.
        MAX_RESPONSIVENESS = 16 // This flag configures the adapter for maximum responsiveness.     
    };

    public interface class IPcapDevice
    {
        property System::String^ Name { System::String^ get(); };
        property System::String^ Description { System::String^ get(); };
        property DeviceFlags^ Flags { DeviceFlags^ get(); };
        property System::Collections::Generic::List<PcapAddress^>^ Addresses { System::Collections::Generic::List<PcapAddress^>^ get(); };

        /// <summary>
        /// Open a generic source in order to capture / send (WinPcap only) traffic. 
        /// </summary>
        /// <param name="snapLen">length of the packet that has to be retained. For each packet received by the filter, only the first 'snaplen' bytes are stored in the buffer and passed to the user application. For instance, snaplen equal to 100 means that only the first 100 bytes of each packet are stored.</param>
        /// <param name="flags">keeps several flags that can be needed for capturing packets.</param>
        /// <param name="flags">read timeout in milliseconds. The read timeout is used to arrange that the read not necessarily return immediately when a packet is seen, but that it waits for some amount of time to allow more packets to arrive and to read multiple packets from the OS kernel in one operation. Not all platforms support a read timeout; on platforms that don't, the read timeout is ignored.</param>
        PcapDeviceHandler^ Open(int snapLen, PcapDeviceOpenFlags flags, int readTimeout);

        /// <summary>
        /// Open a generic source in order to capture / send (WinPcap only) traffic. 
        /// Uses maxmimum snaplen (65536), promiscuous mode and 1 second read timeout.
        /// </summary>
        PcapDeviceHandler^ Open();
    };
}