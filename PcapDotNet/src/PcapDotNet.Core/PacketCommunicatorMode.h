#pragma once

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Working modes for packet communicator.
    /// </summary>
    public enum class PacketCommunicatorMode : int
    {
        /// <summary>Capture working mode.</summary>
        Capture         = 0x0, 

        /// <summary>Statistical working mode.</summary> 
        Statistics      = 0x1, 

        /// <summary>Kernel monitoring mode. </summary>
        KernelMonitor   = 0x2, 

        /// <summary>Kernel dump working mode.</summary>
        KernelDump      = 0x10  
    };
}}