#pragma once

#include "PacketCommunicator.h"
#include "PcapDeclarations.h"
#include <cstdio>

namespace PcapDotNet { namespace Core 
{
    public ref class OfflinePacketCommunicator sealed : PacketCommunicator
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

        ~OfflinePacketCommunicator();

    internal:
        OfflinePacketCommunicator(System::String^ fileName);

    private:
        pcap_t* OpenFile(System::String^ filename);

        static void CloseFile(FILE* file);

        FILE* _file;
    };
}}