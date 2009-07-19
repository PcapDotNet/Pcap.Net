#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PacketSendBuffer : System::IDisposable
    {
    public:
        PacketSendBuffer(unsigned int capacity);

        void Enqueue(Packets::Packet^ packet);

        ~PacketSendBuffer();

    internal:
        void Transmit(pcap_t* pcapDescriptor, bool isSync);

    private:
        pcap_send_queue *_pcapSendQueue;
    };
}}