#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Represents a buffer of packets to be sent.
    /// Note that transmitting a send buffer is much more efficient than performing a series of Send(), because the send buffer is buffered at kernel level drastically decreasing the number of context switches.
    /// </summary>
    public ref class PacketSendBuffer : System::IDisposable
    {
    public:
        /// <summary>
        /// This function allocates a send buffer, i.e. a buffer containing a set of raw packets that will be transimtted on the network with PacketCommunicator.Transmit().
        /// </summary>
        /// <param name="capacity">The size, in bytes, of the buffer, therefore it determines the maximum amount of data that the buffer will contain.</param>
        PacketSendBuffer(unsigned int capacity);

        /// <summary>
        /// Adds a raw packet at the end of the send buffer.
        /// 'Raw packet' means that the sending application will have to include the protocol headers, since every packet is sent to the network 'as is'. The CRC of the packets needs not to be calculated, because it will be transparently added by the network interface.
        /// </summary>
        /// <param name="packet">The packet to be added to the buffer</param>
        /// <exception cref="System::InvalidOperationException">Thrown on failure.</exception>
        void Enqueue(Packets::Packet^ packet);

        /// <summary>
        /// Deletes a send buffer and frees all the memory associated with it.
        /// </summary>
        ~PacketSendBuffer();

    internal:
        void Transmit(pcap_t* pcapDescriptor, bool isSync);

    private:
        pcap_send_queue *_pcapSendQueue;
    };
}}