#pragma once

namespace PcapDotNet 
{
    public ref class PcapTotalStatistics
    {
    public:
        PcapTotalStatistics(unsigned int packetsReceived, unsigned int packetsDroppedByDriver, unsigned int packetsDroppedByInterface, unsigned int packetsCaptured);

        property unsigned int PacketsReceived
        {
            unsigned int get();
        }

        property unsigned int PacketsDroppedByDriver
        {
            unsigned int get();
        }

        property unsigned int PacketsDroppedByInterface
        {
            unsigned int get();
        }

        property unsigned int PacketsCaptured
        {
            unsigned int get();
        }

    private:
        unsigned int _packetsReceived;
        unsigned int _packetsDroppedByDriver;
        unsigned int _packetsDroppedByInterface;
        unsigned int _packetsCaptured;
    };
}