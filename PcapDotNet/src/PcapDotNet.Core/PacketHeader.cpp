#include "PacketHeader.h"
#include "Pcap.h"
#include "Timestamp.h"

using namespace PcapDotNet::Core;
using namespace PcapDotNet::Packets;

// static
void PacketHeader::GetPcapHeader(pcap_pkthdr &header, Packet^ packet)
{
    Timestamp::DateTimeToPcapTimestamp(packet->Timestamp, header.ts);
    header.len = packet->Length;
    header.caplen = packet->Length;
}