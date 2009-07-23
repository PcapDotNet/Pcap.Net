#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Represents a statistics value when running in statistics mode.
    /// </summary>
    public ref class PacketSampleStatistics
    {
    public:
        /// <summary>
        /// The time the statistics was received.
        /// </summary>
        property System::DateTime Timestamp
        {
            System::DateTime get();
        }

        /// <summary>
        /// The number of packets received during the last interval.
        /// </summary>
        property unsigned long AcceptedPackets
        {
            unsigned long get();
        }

        /// <summary>
        /// The number of bytes received during the last interval.
        /// </summary>
        property unsigned long AcceptedBytes
        {
            unsigned long get();
        }

        virtual System::String^ ToString() override;

    internal:
        PacketSampleStatistics(const pcap_pkthdr& packetHeader, const unsigned char* packetData);

    private:
        System::DateTime _timestamp;
        unsigned long _acceptedPackets;
        unsigned long _acceptedBytes;
    };
}}