#include "PacketSendBuffer.h"
#include "PacketHeader.h"
#include "PcapError.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;
using namespace PcapDotNet::Packets;

PacketSendBuffer::PacketSendBuffer(unsigned int capacity)
{
    _pcapSendQueue = pcap_sendqueue_alloc(capacity);
}

int PacketSendBuffer::Length::get()
{
    return _length;
}

void PacketSendBuffer::Enqueue(Packet^ packet)
{
	if (packet == nullptr) 
		throw gcnew ArgumentNullException("packet");

	pcap_pkthdr pcapHeader;
    PacketHeader::GetPcapHeader(pcapHeader, packet);
    pin_ptr<Byte> unmanagedPacketBytes = &packet->Buffer[0];
    if (pcap_sendqueue_queue(_pcapSendQueue, &pcapHeader, unmanagedPacketBytes) != 0)
        throw gcnew InvalidOperationException("Failed enqueueing to queue");

	++_length;
}

PacketSendBuffer::~PacketSendBuffer()
{
    pcap_sendqueue_destroy(_pcapSendQueue);
}

void PacketSendBuffer::Transmit(pcap_t* pcapDescriptor, bool isSync)
{
    unsigned int numBytesTransmitted = pcap_sendqueue_transmit(pcapDescriptor, _pcapSendQueue, isSync);
    if (numBytesTransmitted < _pcapSendQueue->len)
        throw PcapError::BuildInvalidOperation("Failed transmiting packets from queue", pcapDescriptor);

	PcapDotNet::Analysis::PcapDotNetAnalysis::PacketSent(Length);
}
