#pragma once

#include "DeviceAddress.h"
#include "BerkeleyPacketFilter.h"
#include "PacketDumpFile.h"
#include "PacketDeviceOpenAttributes.h"
#include "PacketSampleStatistics.h"
#include "PacketTotalStatistics.h"
#include "PcapDataLink.h"
#include "PacketSendQueue.h"
#include "PacketCommunicatorMode.h"
#include "PacketCommunicatorReceiveResult.h"
#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    public delegate void HandlePacket(Packets::Packet^ packet);
    public delegate void HandleStatistics(PacketSampleStatistics^ statistics);

    /// <summary>
    /// Used to receive and send packets accross the network or to read and write packets to a pcap file.
    /// </summary>
    public ref class PacketCommunicator abstract : System::IDisposable
    {
    public:
        /// <summary>
        /// The link layer of an adapter.
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown when setting the datalink fails.</exception>
        property PcapDataLink DataLink
        {
            PcapDataLink get();
            void set(PcapDataLink value);
        }

        /// <summary>
        /// List of the supported data link types of the interface associated with the packet communicator.
        /// This property is currently unsupported to avoid memory leakage until a bug fix will be released in winpcap.
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown on failure.</exception>
        property System::Collections::ObjectModel::ReadOnlyCollection<PcapDataLink>^ SupportedDataLinks
        {
            System::Collections::ObjectModel::ReadOnlyCollection<PcapDataLink>^ get();
        }

        /// <summary>
        /// The dimension of the packet portion (in bytes) that is delivered to the application. 
        /// </summary>
        property int SnapshotLength
        {
            int get();
        }

        /// <summary>
        /// True if the current file uses a different byte order than the current system. 
        /// </summary>
        property bool IsFileSystemByteOrder
        {
            bool get();
        }

        /// <summary>
        /// The major version number of the pcap library used to write the file.
        /// </summary>
        property int FileMajorVersion
        {
            int get();
        }

        /// <summary>
        /// The minor version number of the pcap library used to write the file.
        /// </summary>
        property int FileMinorVersion
        {
            int get();
        }

        /// <summary>
        /// Return statistics on current capture.
        /// The values represent packet statistics from the start of the run to the time of the call. 
        /// Supported only on live captures, not on offline. No statistics are stored in offline, so no statistics are available when reading from an offline device.
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown if there is an error or the underlying packet capture doesn't support packet statistics.</exception>
        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() = 0;
        }

        /// <summary>
        /// The working mode of the interface.
        /// </summary>
        property PacketCommunicatorMode Mode
        {
            PacketCommunicatorMode get();
            void set(PacketCommunicatorMode value);
        }

        /// <summary>
        /// Switch between blocking and nonblocking mode.
        /// Puts a live communicator into "non-blocking" mode, or takes it out of "non-blocking" mode.
        /// In "non-blocking" mode, an attempt to read from the communicator with ReceiveSomePackets will, if no packets are currently available to be read, return immediately rather than blocking waiting for packets to arrive.
        /// ReceivePacket and ReceivePackets will not work in "non-blocking" mode.
        /// <seealso cref="ReceiveSomePackets"/>
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown if there is an error.</exception>
        property bool NonBlocking
        {
            bool get();
            void set(bool value);
        }

        /// <summary>
        /// Set the size of the kernel buffer associated with an adapter.
        /// If an old buffer was already created with a previous call to pcap_setbuff(), it is deleted and its content is discarded.
        /// LivePacketDevice.Open() creates a 1 MByte buffer by default.
        /// <!--seealso cref="LivePacketDevice::Open"/-->
        /// </summary>
        /// <param name="size">the size of the buffer in bytes</param>
        /// <exception cref="System::InvalidOperationException">Thrown on failure.</exception>
        void SetKernelBufferSize(int size);

        /// <summary>
        /// Set the minumum amount of data received by the kernel in a single call.
        /// Changes the minimum amount of data in the kernel buffer that causes a read from the application to return (unless the timeout expires).
        /// If the value of size is large, the kernel is forced to wait the arrival of several packets before copying the data to the user. 
        /// This guarantees a low number of system calls, i.e. low processor usage, and is a good setting for applications like packet-sniffers and protocol analyzers.
        /// Vice versa, in presence of a small value for this variable, the kernel will copy the packets as soon as the application is ready to receive them.
        /// This is useful for real time applications that need the best responsiveness from the kernel.
        /// </summary>
        /// <param name="size">minimum number of bytes to copy</param>
        /// <exception cref="System::InvalidOperationException">Thrown on failure.</exception>
        void SetKernelMinimumBytesToCopy(int size);

        /// <summary>
        /// Define a sampling method for packet capture.
        /// This function allows applying a sampling method to the packet capture process. 
        /// The mtthod will be applied as soon as the capture starts.
        /// </summary>
        /// <remarks>
        /// Warning: Sampling parameters cannot be changed when a capture is active. These parameters must be applied before starting the capture. If they are applied when the capture is in progress, the new settings are ignored.
        /// Warning: Sampling works only when capturing data on Win32 or reading from a file. It has not been implemented on other platforms. Sampling works on remote machines provided that the probe (i.e. the capturing device) is a Win32 workstation. 
        /// </remarks>
        /// <param name="method">The sampling method to be applied</param>
        void SetSamplingMethod(SamplingMethod^ method);

        /// <summary>
        /// Read a packet from an interface or from an offline capture.
        /// This function is used to retrieve the next available packet, bypassing the callback method traditionally provided.
        /// The method fills the packet parameter with the next captured packet.
        /// <seealso cref="ReceiveSomePackets"/>
        /// <seealso cref="ReceivePackets"/>
        /// </summary>
        /// <param name="packet">The received packet if it was read without problems. null otherwise.</param>
        /// <returns>
        ///   <list type="table">
        ///     <listheader>
        ///         <term>Return value</term>
        ///         <description>description</description>
        ///     </listheader>
        ///     <item><term>Ok</term><description>The packet has been read without problems.</description></item>
        ///     <item><term>Timeout</term><description>The timeout set with LivePacketDevice.Open() has elapsed. In this case the packet out parameter will be null.</description></item>
        ///     <item><term>Eof</term><description>EOF was reached reading from an offline capture. In this case the packet out parameter will be null.</description></item>
        ///   </list>
        /// </returns>
        /// <exception cref="System::InvalidOperationException">Thrown if an error occurred.</exception>
        PacketCommunicatorReceiveResult ReceivePacket([System::Runtime::InteropServices::Out] Packets::Packet^% packet);
        PacketCommunicatorReceiveResult ReceiveSomePackets([System::Runtime::InteropServices::Out] int% countGot, int maxPackets, HandlePacket^ callback);
        PacketCommunicatorReceiveResult ReceivePackets(int count, HandlePacket^ callback);
        
        PacketCommunicatorReceiveResult ReceiveStatistics([System::Runtime::InteropServices::Out] PacketSampleStatistics^% statistics);
        PacketCommunicatorReceiveResult ReceiveStatistics(int count, HandleStatistics^ callback);

        void Break();

        void SendPacket(Packets::Packet^ packet);
        virtual void Transmit(PacketSendBuffer^ sendBuffer, bool isSync) = 0;

        BerkeleyPacketFilter^ CreateFilter(System::String^ filterValue);
        void SetFilter(BerkeleyPacketFilter^ filter);
        void SetFilter(System::String^ filterValue);

        PacketDumpFile^ OpenDump(System::String^ fileName);

        ~PacketCommunicator();

    internal:
        PacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth* auth, 
                           SocketAddress^ netmask);

    protected:
        property pcap_t* PcapDescriptor
        {
            pcap_t* get();
        }

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
            PacketHandler(HandlePacket^ callback, PcapDataLink dataLink)
            {
                _callback = callback;
                _dataLink = dataLink;
            }

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

            property int PacketCounter
            {
                int get();
            }

        private:
            HandlePacket^ _callback;
            PcapDataLink _dataLink;
            int _packetCounter;
        };

        ref class StatisticsHandler
        {
        public:
            StatisticsHandler(HandleStatistics^ callback)
            {
                _callback = callback;
            }

            void Handle(unsigned char *user, const struct pcap_pkthdr *packetHeader, const unsigned char *packetData);

        private:
            HandleStatistics^ _callback;
        };

    private:
        pcap_t* _pcapDescriptor;
        IpV4SocketAddress^ _ipV4Netmask;
        PacketCommunicatorMode _mode;
    };
}}