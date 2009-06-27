#pragma once

namespace PcapDotNet { namespace Core 
{
    public ref class PcapLibrary
    {
        static property System::String^ Version
        {
            System::String^ get();
        }
    };
}}