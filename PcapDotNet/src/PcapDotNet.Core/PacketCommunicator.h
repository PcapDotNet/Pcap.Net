#pragma once

#include "DeviceAddress.h"
#include "BerkeleyPacketFilter.h"
#include "PacketDumpFile.h"
#include "PacketDeviceOpenFlags.h"
#include "PacketSampleStatistics.h"
#include "PacketTotalStatistics.h"
#include "PcapDataLink.h"
#include "PacketSendQueue.h"

namespace PcapDotNet { namespace Core 
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

    public ref class PacketCommunicator : System::IDisposable
    {
    public:
        PacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout, pcap_rmtauth *auth, 
                          SocketAddress^ netmask);

        property PcapDataLink DataLink
        {
            PcapDataLink get();
            void set(PcapDataLink value);
        }

        property System::Collections::ObjectModel::ReadOnlyCollection<PcapDataLink>^ SupportedDataLinks
        {
            System::Collections::ObjectModel::ReadOnlyCollection<PcapDataLink>^ get();
        }

        property int SnapshotLength
        {
            int get();
        }

        property bool IsFileSystemByteOrder
        {
            bool get();
        }

        property int FileMajorVersion
        {
            int get();
        }

        property int FileMinorVersion
        {
            int get();
        }

        property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get();
        }

        property DeviceHandlerMode Mode
        {
            DeviceHandlerMode get();
            void set(DeviceHandlerMode value);
        }

        property bool NonBlocking
        {
            bool get();
            void set(bool value);
        }

        delegate void HandlePacket(Packets::Packet^ packet);
        DeviceHandlerResult GetPacket([System::Runtime::InteropServices::Out] Packets::Packet^% packet);
        DeviceHandlerResult GetSomePackets(int maxPackets, HandlePacket^ callBack, [System::Runtime::InteropServices::Out] int% numPacketsGot);
        DeviceHandlerResult GetPackets(int numPackets, HandlePacket^ callBack);
        
        delegate void HandleStatistics(PacketSampleStatistics^ statistics);
        DeviceHandlerResult GetNextStatistics([System::Runtime::InteropServices::Out] PacketSampleStatistics^% statistics);

        void SendPacket(Packets::Packet^ packet);
        void Transmit(PacketSendQueue^ sendQueue, bool isSync);

        BerkeleyPacketFilter^ CreateFilter(System::String^ filterString);
        void SetFilter(BerkeleyPacketFilter^ filter);
        void SetFilter(System::String^ filterString);

        PacketDumpFile^ OpenDump(System::String^ filename);

        ~PacketCommunicator();

    private:
        static Packets::Packet^ CreatePacket(const pcap_pkthdr& packetHeader, const unsigned char* packetData, Packets::IDataLink^ dataLink);
        static PacketSampleStatistics^ PacketCommunicator::CreateStatistics(const pcap_pkthdr& packetHeader, const unsigned char* packetData);

        DeviceHandlerResult RunPcapNextEx(pcap_pkthdr** packetHeader, const unsigned char** packetData);

        [System::Runtime::InteropServices::UnmanagedFunctionPointer(System::Runtime::InteropServices::CallingConvention::Cdecl)]
        delegate void HandlerDelegate(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

        void AssertMode(DeviceHandlerMode mode);

        property System::String^ ErrorMessage
        {
            System::String^ get();
        }

        System::InvalidOperationException^ BuildInvalidOperation(System::String^ errorMessage);

        ref class PacketHandler
        {
        public:
            PacketHandler(HandlePacket^ callBack, PcapDataLink dataLink)
            {
                _callBack = callBack;
                _dataLink = dataLink;
            }

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

            HandlePacket^ _callBack;
            PcapDataLink _dataLink;
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
}}