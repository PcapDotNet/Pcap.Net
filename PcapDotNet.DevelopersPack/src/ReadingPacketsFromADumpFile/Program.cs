using System;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace ReadingPacketsFromADumpFile
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check command line
            if (args.Length != 1)
            {
                Console.WriteLine("usage: " + Environment.GetCommandLineArgs()[0] + " <filename>");
                return;
            }

            // Create the offline device
            OfflinePacketDevice selectedDevice = new OfflinePacketDevice(args[0]);

            // Open the capture file
            using (PacketCommunicator communicator =
                selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000))                                  // read timeout
            {
                // Read and dispatch packets until EOF is reached
                communicator.ReceivePackets(0, DispatcherHandler);
            }
        }

        private static void DispatcherHandler(Packet packet)
        {
            // print packet timestamp and packet length
            Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);

            // Print the packet
            const int LineLength = 64;
            for (int i = 0; i != packet.Length; ++i)
            {
                Console.Write((packet[i]).ToString("X2"));
                if ((i + 1) % LineLength == 0)
                  Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}