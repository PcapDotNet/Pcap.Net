#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    private ref class Timestamp
    {
    public:
        static void PcapTimestampToDateTime(const timeval& pcapTimestamp, [System::Runtime::InteropServices::Out] System::DateTime% dateTime);
        static void DateTimeToPcapTimestamp(System::DateTime dateTime, timeval& pcapTimestamp);
    };
}}