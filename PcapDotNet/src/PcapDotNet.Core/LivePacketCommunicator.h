#pragma once

#include "PacketCommunicator.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// A network device packet communicator.
    /// </summary>
    public ref class LivePacketCommunicator : PacketCommunicator
    {
    public:
        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() override;
        }

        virtual void Transmit(PacketSendBuffer^ sendBuffer, bool isSync) override;

    internal:
        LivePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth* auth, 
                               SocketAddress^ netmask);

    };
}}