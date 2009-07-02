#include "PacketDumpFile.h"
#include "Timestamp.h"
#include "PacketHeader.h"
#include "MarshalingServices.h"
#include "PcapError.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;
using namespace Packets;

void PacketDumpFile::Dump(Packet^ packet)
{
    pcap_pkthdr header;
    PacketHeader::GetPcapHeader(header, packet);
    std::string unmanagedFilename = MarshalingServices::ManagedToUnmanagedString(_filename);

    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    pcap_dump(reinterpret_cast<unsigned char*>(_pcapDumper), &header, unamangedPacketBytes);
}

void PacketDumpFile::Flush()
{
    if (pcap_dump_flush(_pcapDumper) != 0)
        throw gcnew InvalidOperationException("Failed flusing to file " + _filename);
}

long PacketDumpFile::Position::get()
{
    long position = pcap_dump_ftell(_pcapDumper);
    if (position == -1)
        throw gcnew InvalidOperationException("Failed getting position");
    return position;
}

PacketDumpFile::~PacketDumpFile()
{
    pcap_dump_close(_pcapDumper);
}

// internal

PacketDumpFile::PacketDumpFile(pcap_t* pcapDescriptor, System::String^ filename)
{
    _filename = filename;
    std::string unmanagedString = MarshalingServices::ManagedToUnmanagedString(_filename);
    _pcapDumper = pcap_dump_open(pcapDescriptor, unmanagedString.c_str());
    if (_pcapDumper == NULL)
        throw gcnew InvalidOperationException("Error opening output file " + filename + " Error: " + PcapError::GetErrorMessage(pcapDescriptor));
}
