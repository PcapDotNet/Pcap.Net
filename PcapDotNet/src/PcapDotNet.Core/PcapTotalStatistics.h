#pragma once

namespace PcapDotNet { namespace Core 
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
        
        bool Equals(PcapTotalStatistics^ other);
        virtual bool Equals(System::Object^ other) override;

    private:
        unsigned int _packetsReceived;
        unsigned int _packetsDroppedByDriver;
        unsigned int _packetsDroppedByInterface;
        unsigned int _packetsCaptured;
    };
}}