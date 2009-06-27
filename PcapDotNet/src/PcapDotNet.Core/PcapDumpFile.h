#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PcapDumpFile : System::IDisposable
    {
    public:
        PcapDumpFile(pcap_t* pcapDescriptor, System::String^ filename);

        void Dump(BPacket::Packet^ packet);

        void Flush();

        property long Position
        {
            long get();
        }

        ~PcapDumpFile();

    private:
        pcap_dumper_t* _pcapDumper;
        System::String^ _filename;
    };
}}