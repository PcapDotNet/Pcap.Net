#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet 
{
    public ref class PcapDumpFile
    {
    public:
        PcapDumpFile(pcap_dumper_t* handler, System::String^ filename);

        void Dump(BPacket::Packet^ packet);

    private:
        pcap_dumper_t* _handler;
        System::String^ _filename;
    };
}