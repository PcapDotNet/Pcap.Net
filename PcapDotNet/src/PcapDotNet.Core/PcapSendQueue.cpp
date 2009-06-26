#include "PcapSendQueue.h"
#include "PacketHeader.h"
#include "PcapError.h"
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
    if (pcap_sendqueue_queue(_pcapSendQueue, &pcapHeader, unamangedPacketBytes) != 0)
        throw gcnew InvalidOperationException("Failed enqueueing to SendQueue");
}

void PcapSendQueue::Transmit(PcapDeviceHandler^ deviceHandler, bool isSync)
{
    unsigned int numBytesTransmitted = pcap_sendqueue_transmit(deviceHandler->Descriptor, _pcapSendQueue, isSync);
    if (numBytesTransmitted < _pcapSendQueue->len)
        throw PcapError::BuildInvalidOperation("Failed transmiting packets from queue", deviceHandler->Descriptor);
}

PcapSendQueue::~PcapSendQueue()
{
    pcap_sendqueue_destroy(_pcapSendQueue);
}
