#pragma once

namespace PcapDotNet { namespace Core 
{
    public enum class PacketCommunicatorReceiveResult : int
    {
        Ok,        // if the packet has been read without problems
        Timeout,   // if the timeout set with Open() has elapsed.
        Eof,       // if EOF was reached reading from an offline capture
        BreakLoop, // 
        None
    };
}}