#pragma once

namespace PcapDotNet 
{
    public ref class PcapSampleStatistics
    {
    public:
        PcapSampleStatistics(System::DateTime timestamp, unsigned long acceptedPackets, unsigned long acceptedBytes);

        property System::DateTime Timestamp
        {
            System::DateTime get();
        }

        property unsigned long AcceptedPackets
        {
            unsigned long get();
        }

        property unsigned long AcceptedBytes
        {
            unsigned long get();
        }

        virtual System::String^ ToString() override;

    private:
        System::DateTime _timestamp;
        unsigned long _acceptedPackets;
        unsigned long _acceptedBytes;
    };
}