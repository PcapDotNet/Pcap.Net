#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet 
{
    public ref class PcapSendQueue : System::IDisposable
    {
    public:
        PcapSendQueue(unsigned int size);

        void Enqueue(BPacket::Packet^ packet);

        ~PcapSendQueue();

    private:
        pcap_send_queue *_pcapSendQueue;
    };
}