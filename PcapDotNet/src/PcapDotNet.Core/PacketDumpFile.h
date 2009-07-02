#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PacketDumpFile : System::IDisposable
    {
    public:
        void Dump(Packets::Packet^ packet);

        void Flush();

        property long Position
        {
            long get();
        }

        ~PacketDumpFile();

    internal:
        PacketDumpFile(pcap_t* pcapDescriptor, System::String^ filename);

    private:
        pcap_dumper_t* _pcapDumper;
        System::String^ _filename;
    };
}}