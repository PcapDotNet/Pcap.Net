#pragma once

#include "PacketCommunicator.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// A network device packet communicator.
    /// </summary>
    public ref class LivePacketCommunicator : PacketCommunicator
    {
    public:
        /// <summary>
        /// Statistics on current capture.
        /// The values represent packet statistics from the start of the run to the time of the call. 
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown if there is an error or the underlying packet capture doesn't support packet statistics.</exception>
        virtual property PacketTotalStatistics^ TotalStatistics
        {
            PacketTotalStatistics^ get() override;
        }

        /// <summary>
        /// Send a buffer of packets to the network.
        /// This function transmits the content of a queue to the wire.
        /// <seealso cref="SendPacket"/>
        /// <seealso cref="PacketSendBuffer"/>
        /// </summary>
        /// <param name="sendBuffer">Contains the packets to send.</param>
        /// <param name="isSync">Determines if the send operation must be synchronized: if it is true, the packets are sent respecting the timestamps, otherwise they are sent as fast as possible.</param>
        /// <exception cref="System::InvalidOperationException">An error occurred during the send. The error can be caused by a driver/adapter problem or by an inconsistent/bogus send buffer..</exception>
        /// <remarks>
        ///   <list type="bullet">
        ///     <item>Using this function is more efficient than issuing a series of SendPacket(), because the packets are buffered in the kernel driver, so the number of context switches is reduced. Therefore, expect a better throughput when using Transmit().</item>
        ///     <item>When isSync is true, the packets are synchronized in the kernel with a high precision timestamp. This requires a non-negligible amount of CPU, but allows normally to send the packets with a precision of some microseconds (depending on the accuracy of the performance counter of the machine). Such a precision cannot be reached sending the packets with SendPacket().</item>
        ///   </list>
        /// </remarks>
        virtual void Transmit(PacketSendBuffer^ sendBuffer, bool isSync) override;

    internal:
        LivePacketCommunicator(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth* auth, 
                               SocketAddress^ netmask);

    private:
        static pcap_t* PcapOpen(const char* source, int snapshotLength, PacketDeviceOpenAttributes attributes, int readTimeout, pcap_rmtauth *auth);
    };
}}