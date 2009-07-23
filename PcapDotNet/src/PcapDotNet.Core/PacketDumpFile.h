#pragma once

#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// A file to write packets.
    /// </summary>
    public ref class PacketDumpFile : System::IDisposable
    {
    public:
        /// <summary>
        /// Save a packet to disk.
        /// Outputs a packet to the "savefile" opened with PacketCommunicator.OpenDump().
        /// </summary>
        /// <param name="packet">The packet to write to disk.</param>
        void Dump(Packets::Packet^ packet);

        /// <summary>
        /// Flushes the output buffer to the ``savefile,'' so that any packets written with Dump() but not yet written to the ``savefile'' will be written.
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown on error.</exception>
        void Flush();

        /// <summary>
        /// Return the file position for a "savefile".
        /// Returns the current file position for the "savefile", representing the number of bytes written by PacketCommunicator.OpenDump() and Dump().
        /// </summary>
        /// <exception cref="System::InvalidOperationException">Thrown on error.</exception>
        property long Position
        {
            long get();
        }

        /// <summary>
        /// Closes a savefile.
        /// </summary>
        ~PacketDumpFile();

    internal:
        PacketDumpFile(pcap_t* pcapDescriptor, System::String^ filename);

    private:
        pcap_dumper_t* _pcapDumper;
        System::String^ _filename;
    };
}}