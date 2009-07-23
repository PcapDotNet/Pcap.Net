#pragma once

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// This class holds methods for general pcap library functionality.
    /// </summary>
    public ref class PcapLibrary sealed
    {
    public:
        /// <summary>
        /// The Pcap library version.
        /// </summary>
        static property System::String^ Version
        {
            System::String^ get();
        }

    private:
        [System::Diagnostics::DebuggerNonUserCode]
        PcapLibrary(){}
    };
}}