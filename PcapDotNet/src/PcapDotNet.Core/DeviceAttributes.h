#pragma once

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Attributes of a device.
    /// </summary>
    [System::Flags]
    public enum class DeviceAttributes : System::Int32
    {
        /// <summary>
        /// No attributes apply.
        /// </summary>
        None     = 0x00000000,

        /// <summary>
        /// Interface is loopback.
        /// </summary>
        Loopback = 0x00000001
    };
}}