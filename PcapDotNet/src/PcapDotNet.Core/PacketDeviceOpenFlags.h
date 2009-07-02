#pragma once

namespace PcapDotNet { namespace Core 
{
    [System::Flags]
    public enum class PacketDeviceOpenFlags : System::Int32
    {
        None                    = 0,
        Promiscuous             = 1, // Defines if the adapter has to go in promiscuous mode.
        DataTransferUdpRemote   = 2, // Defines if the data trasfer (in case of a remote capture) has to be done with UDP protocol.
        NoCaptureRemote         = 4, // Defines if the remote probe will capture its own generated traffic.
        NoCaptureLocal          = 8, // Defines if the local adapter will capture its own generated traffic.
        MaximumResponsiveness   = 16 // This flag configures the adapter for maximum responsiveness.     
    };
}}