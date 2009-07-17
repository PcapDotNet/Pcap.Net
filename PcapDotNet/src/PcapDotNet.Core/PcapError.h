#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    private ref class PcapError
    {
    public:
        static System::String^ GetErrorMessage(pcap_t* pcapDescriptor);
        static System::InvalidOperationException^ BuildInvalidOperation(System::String^ errorMessage, pcap_t* pcapDescriptor);

    private:
        PcapError(){}
    };
}}