#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet 
{
    private ref class PcapError
    {
    public:
        static System::String^ GetErrorMessage(pcap_t* pcapDescriptor);
        static System::InvalidOperationException^ BuildInvalidOperation(System::String^ errorMessage, pcap_t* pcapDescriptor);
    };
}