#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PacketTimestamp sealed
    {
    public:
        /// <summary>
        /// The minimum legal timestamp to put in a packet.
        /// </summary>
        static property System::DateTime MinimumPacketTimestamp
        {
            System::DateTime get();
        }

        /// <summary>
        /// The maximum legal timestamp to put in a packet.
        /// </summary>
        static property System::DateTime MaximumPacketTimestamp
        {
            System::DateTime get();
        }

    internal:
        static void PcapTimestampToDateTime(const timeval& pcapTimestamp, [System::Runtime::InteropServices::Out] System::DateTime% dateTime);
        static void DateTimeToPcapTimestamp(System::DateTime dateTime, timeval& pcapTimestamp);

    private:
        static PacketTimestamp() { Initialize(); }
        PacketTimestamp(){}
        static void Initialize();

        static System::DateTime _minimumPacketTimestamp;
        static System::DateTime _maximumPacketTimestamp;
    };
}}