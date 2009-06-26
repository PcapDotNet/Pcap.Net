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
        
        delegate void HandleStatistics(PcapStatistics^ statistics);
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
        static PcapStatistics^ PcapDeviceHandler::CreateStatistics(const pcap_pkthdr& packetHeader, const unsigned char* packetData);

        DeviceHandlerResult RunPcapNextEx(pcap_pkthdr** packetHeader, const unsigned char** packetData);

        [System::Runtime::InteropServices::UnmanagedFunctionPointer(System::Runtime::InteropServices::CallingConvention::Cdecl)]
        delegate void HandlerDelegate(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

        DeviceHandlerResult RunPcapDispatch(int maxInstances, HandlerDelegate^ callBack, [System::Runtime::InteropServices::Out] int% numInstancesGot);

        void AssertMode(DeviceHandlerMode mode);

        property System::String^ ErrorMessage
        {
            System::String^ get();
        }

        System::InvalidOperationException^ BuildInvalidOperation(System::String^ errorMessage);

        ref class PacketHandler
        {
        public:
            PacketHandler(HandlePacket^ callBack)
            {
                _callBack = callBack;
            }

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

            HandlePacket^ _callBack;
        };

        ref class StatisticsHandler
        {
        public:
            StatisticsHandler(HandleStatistics^ callBack)
            {
                _callBack = callBack;
            }

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

            HandleStatistics^ _callBack;
        };

    private:
        pcap_t* _pcapDescriptor;
        IpV4SocketAddress^ _ipV4Netmask;
        DeviceHandlerMode _mode;
    };
}
