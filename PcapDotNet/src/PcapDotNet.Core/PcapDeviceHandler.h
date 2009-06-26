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
        Ok      = 1,  // if the packet has been read without problems
        Timeout = 0,  // if the timeout set with Open() has elapsed.
        Error   = -1, // if an error occurred
        Eof     = -2  // if EOF was reached reading from an offline capture
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
        //PcapDeviceHandler(pcap_t* pcapDescriptor, SocketAddress^ netmask);

        property DeviceHandlerMode Mode
        {
            DeviceHandlerMode get();
            void set(DeviceHandlerMode value);
        }

        DeviceHandlerResult GetNextPacket([System::Runtime::InteropServices::Out] BPacket::Packet^% packet);
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
        pcap_t* _pcapDescriptor;
        IpV4SocketAddress^ _ipV4Netmask;
        DeviceHandlerMode _mode;
    };
}