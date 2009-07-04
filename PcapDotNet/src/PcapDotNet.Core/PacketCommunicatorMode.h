#pragma once

namespace PcapDotNet { namespace Core 
{
    public enum class PacketCommunicatorMode : int
    {
        Capture         = 0x0, // Capture working mode.  
        Statistics      = 0x1, // Statistical working mode. 
        KernelMonitor   = 0x2, // Kernel monitoring mode. 
        KernelDump      = 0x10 // Kernel dump working mode. 
    };
}}