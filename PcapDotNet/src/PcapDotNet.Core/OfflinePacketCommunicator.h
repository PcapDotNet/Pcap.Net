#pragma once

#include "PacketCommunicator.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class OfflinePacketCommunicator : PacketCommunicator
    {
    public:
        /// <summary>
        /// TotalStatistics is not supported on offline captures.
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown always.</exception>
        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() override;
        }

        /// <summary>
        /// Transmit is not supported on offline captures.
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown always.</exception>
        virtual void Transmit(PacketSendBuffer^ sendBuffer, bool isSync) override;

    internal:
        OfflinePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth* auth);
    };
}}
