#include "PcapDumpFile.h"
#include "Timestamp.h"
#include "PacketHeader.h"
#include "MarshalingServices.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet;
using namespace BPacket;

PcapDumpFile::PcapDumpFile(pcap_dumper_t* handler, System::String^ filename)
{
    _handler = handler;
    _filename = filename;
}

void PcapDumpFile::Dump(Packet^ packet)
{
    pcap_pkthdr header;
    PacketHeader::GetPcapHeader(header, packet);
    std::string unmanagedFilename = MarshalingServices::ManagedToUnmanagedString(_filename);

    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    pcap_dump(reinterpret_cast<unsigned char*>(_handler), &header, unamangedPacketBytes);
}