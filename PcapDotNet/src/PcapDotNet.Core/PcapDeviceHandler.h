#pragma once

#include "PcapAddress.h"
#include "BpfFilter.h"
#include "PcapDumpFile.h"
#include "PcapDeviceOpenFlags.h"
#include "PcapStatistics.h"

namespace PcapDotNet 
{
    public enum class DeviceHandlerResult : int
    {
        Ok,       // if the packet has been read without problems
        Timeout,  // if the timeout set with Open() has elapsed.
        Error,    // if an error occurred
        Eof,      // if EOF was reached reading from an offline capture
        BreakLoop // 
    };

    public enum class DeviceHandlerMode : int
    {
        Capture         = 0x0, // Capture working mode.  
        Statistics      = 0x1, // Statistical working mode. 
        KernelMonitor   = 0x2, // Kernel monitoring mode. 
        KernelDump      = 0x10 // Kernel dump working mode. 
    };

    public ref class PcapDeviceHandler : System::IDisposable
    {
    public:
        PcapDeviceHandler(const char* source, int snapLen, PcapDeviceOpenFlags flags, int readTimeout, pcap_rmtauth *auth, 
                          SocketAddress^ netmask);

        property DeviceHandlerMode Mode
        {
            DeviceHandlerMode get();
            void set(DeviceHandlerMode value);
        }

        delegate void HandlePacket(BPacket::Packet^ packet);

        DeviceHandlerResult GetNextPacket([System::Runtime::InteropServices::Out] BPacket::Packet^% packet);
        DeviceHandlerResult GetNextPackets(int maxPackets, HandlePacket^ callBack, [System::Runtime::InteropServices::Out] int% numPacketsGot);
        DeviceHandlerResult GetNextStatistics([System::Runtime::InteropServices::Out] PcapStatistics^% statistics);

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
        static BPacket::Packet^ CreatePacket(const pcap_pkthdr& packetHeader, const unsigned char* packetData);

        DeviceHandlerResult RunPcapNextEx(pcap_pkthdr** packetHeader, const unsigned char** packetData);

        ref class PacketHandler
        {
        public:
            PacketHandler(HandlePacket^ callBack)
            {
                _callBack = callBack;
            }

            [System::Runtime::InteropServices::UnmanagedFunctionPointer(System::Runtime::InteropServices::CallingConvention::Cdecl)]
            delegate void Delegate(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

            HandlePacket^ _callBack;
        };

    private:
        pcap_t* _pcapDescriptor;
        IpV4SocketAddress^ _ipV4Netmask;
        DeviceHandlerMode _mode;
    };
}
