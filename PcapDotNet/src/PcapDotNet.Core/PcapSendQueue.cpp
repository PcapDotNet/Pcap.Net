#include "PcapSendQueue.h"
#include "Pcap.h"

using namespace PcapDotNet;
using namespace BPacket;

PcapSendQueue::PcapSendQueue(unsigned int size)
{
    _pcapSendQueue = pcap_sendqueue_alloc(size);
}

void PcapSendQueue::Enqueue(Packet^ packet)
{

    //pcap_sendqueue_queue(_pcapSendQueue, 
}

PcapSendQueue::~PcapSendQueue()
{
    pcap_sendqueue_destroy(_pcapSendQueue);
}
