#pragma once

#include "DeviceAddress.h"
#include "BerkeleyPacketFilter.h"
#include "PacketDumpFile.h"
#include "PacketDeviceOpenFlags.h"
#include "PacketSampleStatistics.h"
#include "PacketTotalStatistics.h"
#include "PcapDataLink.h"
#include "PacketSendQueue.h"
#include "PacketCommunicatorMode.h"
#include "PacketCommunicatorReceiveResult.h"
#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Used to receive and send packets accross the network or to read and write packets to a pcap file.
    /// </summary>
    public ref class PacketCommunicator abstract : System::IDisposable
    {
    public:
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

        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() = 0;
        }

        property PacketCommunicatorMode Mode
        {
            PacketCommunicatorMode get();
            void set(PacketCommunicatorMode value);
        }

        property bool NonBlocking
        {
            bool get();
            void set(bool value);
        }

        void SetKernelBufferSize(int size);

        void SetKernelMinimumBytesToCopy(int size);

        void SetSamplingMethod(SamplingMethod^ method);

        delegate void HandlePacket(Packets::Packet^ packet);
        PacketCommunicatorReceiveResult ReceivePacket([System::Runtime::InteropServices::Out] Packets::Packet^% packet);
        PacketCommunicatorReceiveResult ReceiveSomePackets([System::Runtime::InteropServices::Out] int% numPacketsGot, int maxPackets, HandlePacket^ callBack);
        PacketCommunicatorReceiveResult ReceivePackets(int numPackets, HandlePacket^ callBack);
        
        delegate void HandleStatistics(PacketSampleStatistics^ statistics);
        PacketCommunicatorReceiveResult ReceiveStatistics([System::Runtime::InteropServices::Out] PacketSampleStatistics^% statistics);
        PacketCommunicatorReceiveResult ReceiveStatistics(int numStatistics, HandleStatistics^ callBack);

        void Break();

        void SendPacket(Packets::Packet^ packet);
        virtual void Transmit(PacketSendQueue^ sendQueue, bool isSync) = 0;

        BerkeleyPacketFilter^ CreateFilter(System::String^ filterString);
        void SetFilter(BerkeleyPacketFilter^ filter);
        void SetFilter(System::String^ filterString);

        PacketDumpFile^ OpenDump(System::String^ filename);

        ~PacketCommunicator();

    internal:
        PacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenFlags flags, int readTimeout, pcap_rmtauth* auth, 
                           SocketAddress^ netmask);

    protected:
        System::InvalidOperationException^ BuildInvalidOperation(System::String^ errorMessage);

    private:
        static Packets::Packet^ CreatePacket(const pcap_pkthdr& packetHeader, const unsigned char* packetData, Packets::IDataLink^ dataLink);
        static PacketSampleStatistics^ PacketCommunicator::CreateStatistics(const pcap_pkthdr& packetHeader, const unsigned char* packetData);

        PacketCommunicatorReceiveResult RunPcapNextEx(pcap_pkthdr** packetHeader, const unsigned char** packetData);

        [System::Runtime::InteropServices::UnmanagedFunctionPointer(System::Runtime::InteropServices::CallingConvention::Cdecl)]
        delegate void HandlerDelegate(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

        void AssertMode(PacketCommunicatorMode mode);

        ref class PacketHandler
        {
        public:
            PacketHandler(HandlePacket^ callBack, PcapDataLink dataLink)
            {
                _callBack = callBack;
                _dataLink = dataLink;
            }

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

            property int PacketCounter
            {
                int get();
            }

        private:
            HandlePacket^ _callBack;
            PcapDataLink _dataLink;
            int _packetCounter;
        };

        ref class StatisticsHandler
        {
        public:
            StatisticsHandler(HandleStatistics^ callBack)
            {
                _callBack = callBack;
            }

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

        private:
            HandleStatistics^ _callBack;
        };

    protected:
        pcap_t* _pcapDescriptor;

    private:
        IpV4SocketAddress^ _ipV4Netmask;
        PacketCommunicatorMode _mode;
    };
}}