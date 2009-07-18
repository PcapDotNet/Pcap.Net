#pragma once

namespace PcapDotNet { namespace Core 
{
    public ref class PcapLibrary sealed
    {
    public:
        static property System::String^ Version
        {
            System::String^ get();
        }

    private:
        [System::Diagnostics::DebuggerNonUserCode]
        PcapLibrary(){}
    };
}}