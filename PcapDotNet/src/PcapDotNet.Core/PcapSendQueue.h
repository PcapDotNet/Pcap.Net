#pragma once

#include "PcapDeviceHandler.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PcapSendQueue : System::IDisposable
    {
    public:
        PcapSendQueue(unsigned int capacity);

        void Enqueue(BPacket::Packet^ packet);

        void Transmit(PcapDeviceHandler^ deviceHandler, bool isSync);

        ~PcapSendQueue();

    private:
        pcap_send_queue *_pcapSendQueue;
    };
}}