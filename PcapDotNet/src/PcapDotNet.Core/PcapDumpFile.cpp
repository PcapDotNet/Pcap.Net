#include "PcapDumpFile.h"

#include <stdio.h>
#include <pcap.h>

#include "Timestamp.h"
#include "MarshalingServices.h"

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
    Timestamp::DateTimeToPcapTimestamp(packet->Timestamp, header.ts);
    header.len = packet->Length;
    header.caplen = packet->Length;
    std::string unmanagedFilename = MarshalingServices::ManagedToUnmanagedString(_filename);

    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    pcap_dump(reinterpret_cast<unsigned char*>(_handler), &header, unamangedPacketBytes);
}