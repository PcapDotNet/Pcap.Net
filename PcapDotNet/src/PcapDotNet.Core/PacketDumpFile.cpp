#include "PacketDumpFile.h"
#include "PacketTimestamp.h"
#include "PacketHeader.h"
#include "MarshalingServices.h"
#include "PcapError.h"
#include "Pcap.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace PcapDotNet::Core;
using namespace PcapDotNet::Packets;

// static
void PacketDumpFile::Dump(String^ fileName, PcapDataLink dataLink, int snapshotLength, IEnumerable<Packet^>^ packets)
{
	if (packets == nullptr) 
		throw gcnew ArgumentNullException("packets");

    pcap_t* pcapDescriptor = pcap_open_dead(dataLink.Value, snapshotLength);
    if (pcapDescriptor == NULL)
        throw gcnew InvalidOperationException("Unable to open open a dead capture");

    try
    {
        PacketDumpFile^ dumpFile = gcnew PacketDumpFile(pcapDescriptor, fileName);
        try
        {
            for each (Packet^ packet in packets)
            {
                dumpFile->Dump(packet);
            }
        }
        finally
        {
            dumpFile->~PacketDumpFile();
        }
    }
    finally
    {
        pcap_close(pcapDescriptor);
    }
}

void PacketDumpFile::Dump(Packet^ packet)
{
	if (packet == nullptr) 
		throw gcnew ArgumentNullException("packet");

	pcap_pkthdr header;
    PacketHeader::GetPcapHeader(header, packet);
    std::string unmanagedFilename = MarshalingServices::ManagedToUnmanagedString(_filename);

    pin_ptr<Byte> unamangedPacketBytes = &packet->Buffer[0];
    pcap_dump(reinterpret_cast<unsigned char*>(_pcapDumper), &header, unamangedPacketBytes);
}

void PacketDumpFile::Flush()
{
    if (pcap_dump_flush(_pcapDumper) != 0)
		throw gcnew InvalidOperationException("Failed flushing to file " + _filename);
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
