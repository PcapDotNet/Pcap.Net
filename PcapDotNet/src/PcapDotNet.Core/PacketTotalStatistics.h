#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Statistics on capture from the start of the run. 
    /// </summary>
    public ref class PacketTotalStatistics : System::IEquatable<PacketTotalStatistics^>
    {
    public:
        /// <summary>
        /// Number of packets transited on the network.
        /// </summary>
        property unsigned int PacketsReceived
        {
            unsigned int get();
        }

        /// <summary>
        /// Number of packets dropped by the driver.
        /// </summary>
        property unsigned int PacketsDroppedByDriver
        {
            unsigned int get();
        }

        /// <summary>
        /// number of packets dropped by the interface.
        /// </summary>
        property unsigned int PacketsDroppedByInterface
        {
            unsigned int get();
        }

        /// <summary>
        /// Win32 specific. Number of packets captured, i.e number of packets that are accepted by the filter, that find place in the kernel buffer and therefore that actually reach the application.
        /// </summary>
        property unsigned int PacketsCaptured
        {
            unsigned int get();
        }
        
        virtual bool Equals(PacketTotalStatistics^ other);
        virtual bool Equals(System::Object^ obj) override;

        virtual int GetHashCode() override;

        virtual System::String^ ToString() override;

    internal:
        PacketTotalStatistics(const pcap_stat& statistics, int statisticsSize);

    private:
        unsigned int _packetsReceived;
        unsigned int _packetsDroppedByDriver;
        unsigned int _packetsDroppedByInterface;
        unsigned int _packetsCaptured;
    };
}}