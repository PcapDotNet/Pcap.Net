#pragma once

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// The different return values when receiving from a packet communicator.
    /// </summary>
    public enum class PacketCommunicatorReceiveResult : int
    {
        /// <summary>The packets/statistics have been read without problems.</summary>
        Ok,   
        
        /// <summary>The timeout set with Open() has elapsed when trying to read packets.</summary>
        Timeout,   
        
        /// <summary>EOF was reached reading from an offline capture.</summary>
        Eof,       

        /// <summary>The loop has been broken by a call to Break() before all the requested packets could be read.</summary>
        BreakLoop,

        /// <summary>This return value should never be returned</summary>
        None
    };
}}