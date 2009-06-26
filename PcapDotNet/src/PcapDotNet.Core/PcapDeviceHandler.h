#pragma once

#include "PcapAddress.h"
#include "BpfFilter.h"
#include "PcapDumpFile.h"

namespace PcapDotNet 
{
    public enum class DeviceHandlerResult
    {
        Ok      = 1,  // if the packet has been read without problems
        Timeout = 0,  // if the timeout set with Open() has elapsed.
        Error   = -1, // if an error occurred
        Eof     = -2  // if EOF was reached reading from an offline capture
    };

    public ref class PcapDeviceHandler : System::IDisposable
    {
    public:
        PcapDeviceHandler(pcap_t* pcapDescriptor, SocketAddress^ netmask);

        DeviceHandlerResult GetNextPacket([System::Runtime::InteropServices::Out] BPacket::Packet^% packet);

        void SendPacket(BPacket::Packet^ packet);

        BpfFilter^ CreateFilter(System::String^ filterString);
        void SetFilter(BpfFilter^ filter);
        void SetFilter(System::String^ filterString);

        PcapDumpFile^ OpenDump(System::String^ filename);

        ~PcapDeviceHandler();

    internal:
        property pcap_t* Descriptor
        {
            pcap_t* get();
        }

    private:
        pcap_t* _pcapDescriptor;
        IpV4SocketAddress^ _ipV4Netmask;
    };
}