#pragma once

#include "IpV4socketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// A packet filter, converting a high level filtering expression (see <see href="http://www.winpcap.org/docs/docs_40_2/html/group__language.html">WinPcap Filtering expression syntax</see>) in a program that can be interpreted by the kernel-level filtering engine. 
    /// The user must dispose instances of this class to deallocate resources.
    /// </summary>
    public ref class BerkeleyPacketFilter : System::IDisposable
    {
    public:
        /// <summary>
        /// Compile a packet filter without the need of opening an adapter. 
        /// This constructor converts a high level filtering expression (see <see href="http://www.winpcap.org/docs/docs_40_2/html/group__language.html">WinPcap Filtering expression syntax</see>) in a program that can be interpreted by the kernel-level filtering engine. 
        /// </summary>
        /// <param name="filterValue">A high level filtering expression (see <see href="http://www.winpcap.org/docs/docs_40_2/html/group__language.html">WinPcap Filtering expression syntax</see>)</param>
        /// <param name="snapshotLength">Length of the packet that has to be retained of the communicator this filter will be applied on.</param>
        /// <param name="kind">The link layer of an adapter that this filter will apply upon.</param>
        /// <param name="netmask">Specifies the IPv4 netmask of the network on which packets are being captured; it is used only when checking for IPv4 broadcast addresses in the filter program. If the netmask of the network on which packets are being captured isn't known to the program, or if packets are being captured on the Linux "any" pseudo-interface that can capture on more than one network, null can be supplied; tests for IPv4 broadcast addreses won't be done correctly, but all other tests in the filter program will be OK.</param>
        /// <exception cref="System::ArgumentException">Indicates an error. Probably caused by bad syntax.</exception>
        /// <remarks>
        /// If the purpose of this filter is to apply it on a communicator and not to test packets in memory, it would be simpler to call to PacketCommunicator.CreateFilter() or to directly call PacketCommunicator.SetFilter().
        /// </remarks>
        BerkeleyPacketFilter(System::String^ filterValue, int snapshotLength, Packets::DataLinkKind kind, IpV4SocketAddress^ netmask);

        /// <summary>
        /// Compile a packet filter without the need of opening an adapter. 
        /// This constructor converts a high level filtering expression (see <see href="http://www.winpcap.org/docs/docs_40_2/html/group__language.html">WinPcap Filtering expression syntax</see>) in a program that can be interpreted by the kernel-level filtering engine. 
        /// Assumes the netmask of the network on which packets are being captured isn't known to the program, or that packets are being captured on the Linux "any" pseudo-interface that can capture on more than one network.
        /// Tests for IPv4 broadcast addreses won't be done correctly, but all other tests in the filter program will be OK.
        /// </summary>
        /// <param name="filterValue">A high level filtering expression (see <see href="http://www.winpcap.org/docs/docs_40_2/html/group__language.html">WinPcap Filtering expression syntax</see>)</param>
        /// <param name="snapshotLength">Length of the packet that has to be retained of the communicator this filter will be applied on.</param>
        /// <param name="kind">The link layer of an adapter that this filter will apply upon.</param>
        /// <exception cref="System::ArgumentException">Indicates an error. Probably caused by bad syntax.</exception>
        /// <remarks>
        /// If the purpose of this filter is to apply it on a communicator and not to test packets in memory, it would be simpler to call to PacketCommunicator.CreateFilter() or to directly call PacketCommunicator.SetFilter().
        /// </remarks>
        BerkeleyPacketFilter(System::String^ filterValue, int snapshotLength, Packets::DataLinkKind kind);

        /// <summary>
        /// Returns if a given filter applies to an offline packet.
        /// This method is used to apply a filter to a packet that is currently in memory. 
        /// This process does not need to open an adapter; we need just to create the proper filter (by settings parameters like the snapshot length, or the link-layer type) by means of the Pcap.
        /// The current API of libpcap does not allow to receive a packet and to filter the packet after it has been received. However, this can be useful in case you want to filter packets in the application, instead of into the receiving process. This function allows you to do the job.
        /// </summary>
        /// <param name="snapshotLength">The length of the bytes that are currently available into the packet if the packet satisfies the filter, 0 otherwise.</param>
        /// <param name="packet">The packet that has to be filtered.</param>
        /// <returns>
        /// True iff the given packet satisfies the filter.
        /// </returns>
        bool Test([System::Runtime::InteropServices::Out] int% snapshotLength, Packets::Packet^ packet);

        /// <summary>
        /// Returns if a given filter applies to an offline packet.
        /// This method is used to apply a filter to a packet that is currently in memory. 
        /// This process does not need to open an adapter; we need just to create the proper filter (by settings parameters like the snapshot length, or the link-layer type) by means of the Pcap.
        /// The current API of libpcap does not allow to receive a packet and to filter the packet after it has been received. However, this can be useful in case you want to filter packets in the application, instead of into the receiving process. This function allows you to do the job.
        /// </summary>
        /// <param name="packet">The packet that has to be filtered.</param>
        /// <returns>
        /// True iff the given packet satisfies the filter.
        /// </returns>
        bool Test(Packets::Packet^ packet);

        /// <summary>
        /// Free a filter.
        /// Used to free up allocated memory when that BPF program is no longer needed, for example after it has been made the filter program for a packet communicator by a call to PacketCommunicator.SetFilter().
        /// </summary>
        ~BerkeleyPacketFilter(); // IDisposable

    internal:
        BerkeleyPacketFilter(pcap_t* pcapDescriptor, System::String^ filterString, IpV4SocketAddress^ netmask);
        void SetFilter(pcap_t* pcapDescriptor);

    private:
        void Initialize(System::String^ filterString, int snapshotLength, Packets::DataLinkKind kind, IpV4SocketAddress^ netmask);
        void Initialize(pcap_t* pcapDescriptor, System::String^ filterString, IpV4SocketAddress^ netmask);

    private:
        bpf_program* _bpf;
    };
}}