#include "PcapSendQueue.h"
#include "PacketHeader.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet;
using namespace BPacket;

PcapSendQueue::PcapSendQueue(unsigned int capacity)
{
    _pcapSendQueue = pcap_sendqueue_alloc(capacity);
}

void PcapSendQueue::Enqueue(Packet^ packet)
{
    pcap_pkthdr pcapHeader;
    PacketHeader::GetPcapHeader(pcapHeader, packet);
    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    if (pcap_sendqueue_queue(_pcapSendQueue, &pcapHeader, unamangedPacketBytes) == -1)
        throw gcnew InvalidOperationException("Failed enqueue packet");
}

void PcapSendQueue::Transmit(PcapDeviceHandler^ deviceHandler, bool isSync)
{
    unsigned int numBytesTransmitted = pcap_sendqueue_transmit(deviceHandler->Descriptor, _pcapSendQueue, isSync);
    if (numBytesTransmitted < _pcapSendQueue->len)
    {
        throw gcnew InvalidOperationException(String::Format("An error occurred sending the packets: %s. Only %d bytes were sent", gcnew String(pcap_geterr(deviceHandler->Descriptor)), numBytesTransmitted));
    }
}

PcapSendQueue::~PcapSendQueue()
{
    pcap_sendqueue_destroy(_pcapSendQueue);
}
