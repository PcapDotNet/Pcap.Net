#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PcapDumpFile : System::IDisposable
    {
    public:
        void Dump(BPacket::Packet^ packet);

        void Flush();

        property long Position
        {
            long get();
        }

        ~PcapDumpFile();

    internal:
        PcapDumpFile(pcap_t* pcapDescriptor, System::String^ filename);

    private:
        pcap_dumper_t* _pcapDumper;
        System::String^ _filename;
    };
}}