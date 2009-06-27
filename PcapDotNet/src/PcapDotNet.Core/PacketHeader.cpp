#include "PacketHeader.h"
#include "Pcap.h"
#include "Timestamp.h"

using namespace PcapDotNet::Core;

// static
void PacketHeader::GetPcapHeader(pcap_pkthdr &header, BPacket::Packet^ packet)
{
    Timestamp::DateTimeToPcapTimestamp(packet->Timestamp, header.ts);
    header.len = packet->Length;
    header.caplen = packet->Length;
}