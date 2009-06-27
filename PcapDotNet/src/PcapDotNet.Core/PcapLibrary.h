#pragma once

namespace PcapDotNet 
{
    public ref class PcapLibrary
    {
        static property System::String^ Version
        {
            System::String^ get();
        }
    };
}