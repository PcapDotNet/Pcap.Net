#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PcapSendQueue : System::IDisposable
    {
    public:
        PcapSendQueue(unsigned int capacity);

        void Enqueue(BPacket::Packet^ packet);

        ~PcapSendQueue();

    internal:
        void Transmit(pcap_t* pcapDescriptor, bool isSync);

    private:
        pcap_send_queue *_pcapSendQueue;
    };
}}