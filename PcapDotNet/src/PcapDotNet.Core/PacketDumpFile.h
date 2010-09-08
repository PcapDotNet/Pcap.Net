#pragma once

#include "PcapDeclarations.h"
#include "PcapDataLink.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// A file to write packets.
    /// </summary>
    public ref class PacketDumpFile : System::IDisposable
    {
    public:
        /// <summary>
        /// Creates a dump file and saves the given packets to disk.
        /// This method is useful when you've got packets to save but no device.
        /// </summary>
        /// <param name="fileName">The name of the dump file.</param>
        /// <param name="dataLink">The data link of the packets saved globally in the dump file.</param>
        /// <param name="snapshotLength">The dimension of the packet portion (in bytes) that is used when writing the packets. 65536 guarantees that the whole packet will be captured on all the link layers.</param>
        /// <param name="packets">The packets to save to the dump file.</param>
        static void Dump(System::String^ fileName, PcapDataLink dataLink, int snapshotLength, System::Collections::Generic::IEnumerable<Packets::Packet^>^ packets);
        
		static void Dump(System::String^ fileName, PcapDotNet::Packets::DataLinkKind dataLink, int snapshotLength, System::Collections::Generic::IEnumerable<Packets::Packet^>^ packets);

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