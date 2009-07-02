#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PacketSendQueue : System::IDisposable
    {
    public:
        PacketSendQueue(unsigned int capacity);

        void Enqueue(BPacket::Packet^ packet);

        ~PacketSendQueue();

    internal:
        void Transmit(pcap_t* pcapDescriptor, bool isSync);

    private:
        pcap_send_queue *_pcapSendQueue;
    };
}}