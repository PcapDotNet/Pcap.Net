#pragma once

#include "PacketCommunicator.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class OfflinePacketCommunicator : PacketCommunicator
    {
    public:
        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() override;
        }

    internal:
        OfflinePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout, pcap_rmtauth* auth);
    };
}}
