#pragma once

#include "PacketCommunicator.h"

namespace PcapDotNet { namespace Core 
{
    public ref class LivePacketCommunicator : PacketCommunicator
    {
    public:
        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() override;
        }

        virtual void Transmit(PacketSendQueue^ sendQueue, bool isSync) override;

    internal:
        LivePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout, pcap_rmtauth* auth, 
                               SocketAddress^ netmask);

    };
}}