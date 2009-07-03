#pragma once

#include "PacketCommunicator.h"

namespace PcapDotNet { namespace Core 
{
    public ref class OnlinePacketCommunicator : PacketCommunicator
    {
    public:
        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() override;
        }

    internal:
        OnlinePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout, pcap_rmtauth* auth, 
                                 SocketAddress^ netmask);

    };
}}