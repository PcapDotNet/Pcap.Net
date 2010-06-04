using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PcapDotNet.Core;
using PcapDotNet.Packets;

namespace SendingPacketsUsingSendBuffer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Send anonymous statistics about the usage of Pcap.Net
            PcapDotNet.Analysis.PcapDotNetAnalysis.OptIn = true;

            // Check the validity of the command line
            if (args.Length == 0 || args.Length > 2)
            {
                Usage();
                return;
            }

            // Retrieve the device list from the local machine
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
            {
                Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
                Console.Write((i + 1) + ". " + device.Name);
                if (device.Description != null)
                    Console.WriteLine(" (" + device.Description + ")");
                else
                    Console.WriteLine(" (No description available)");
            }

            int deviceIndex = 0;
            do
            {
                Console.WriteLine("Enter the interface number (1-" + allDevices.Count + "):");
                string deviceIndexString = Console.ReadLine();
                if (!int.TryParse(deviceIndexString, out deviceIndex) ||
                    deviceIndex < 1 || deviceIndex > allDevices.Count)
                {
                    deviceIndex = 0;
                }
            } while (deviceIndex == 0);

            // Take the selected adapter
            PacketDevice selectedOutputDevice = allDevices[deviceIndex - 1];

            // Retrieve the length of the capture file
            long capLength = new FileInfo(args[0]).Length;

            // Chek if the timestamps must be respected
            bool isSync = (args.Length == 2 && args[1][0] == 's');

            // Open the capture file
            OfflinePacketDevice selectedInputDevice = new OfflinePacketDevice(args[0]);

            using (PacketCommunicator inputCommunicator =
                selectedInputDevice.Open(65536, // portion of the packet to capture
                // 65536 guarantees that the whole packet will be captured on all the link layers
                                         PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                         1000)) // read timeout
            {
                using (PacketCommunicator outputCommunicator =
                    selectedOutputDevice.Open(100, PacketDeviceOpenAttributes.Promiscuous, 1000))
                {
                    // Check the MAC type
                    if (inputCommunicator.DataLink != outputCommunicator.DataLink)
                    {
                        Console.WriteLine(
                            "Warning: the datalink of the capture differs from the one of the selected interface.");
                        Console.WriteLine("Press a key to continue, or CTRL+C to stop.");
                        Console.ReadKey();
                    }

                    // Allocate a send buffer
                    using (PacketSendBuffer sendBuffer = new PacketSendBuffer((uint)capLength))
                    {
                        // Fill the buffer with the packets from the file
                        int numPackets = 0;
                        Packet packet;
                        while (inputCommunicator.ReceivePacket(out packet) == PacketCommunicatorReceiveResult.Ok)
                        {
                            sendBuffer.Enqueue(packet);
                            ++numPackets;
                        }

                        // Transmit the queue
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        long startTimeMs = stopwatch.ElapsedMilliseconds;
                        Console.WriteLine("Start Time: " + startTimeMs);
                        outputCommunicator.Transmit(sendBuffer, isSync);
                        long endTimeMs = stopwatch.ElapsedMilliseconds;
                        Console.WriteLine("End Time: " + endTimeMs);
                        long elapsedTimeMs = endTimeMs - startTimeMs;
                        Console.WriteLine("Elapsed Time: " + elapsedTimeMs);
                        double averagePacketsPerSecond = elapsedTimeMs == 0 ? double.MaxValue : (double)numPackets / elapsedTimeMs * 1000;

                        Console.WriteLine("Elapsed time: " + elapsedTimeMs + " ms");
                        Console.WriteLine("Total packets generated = " + numPackets);
                        Console.WriteLine("Average packets per second = " + averagePacketsPerSecond);
                        Console.WriteLine();
                    }
                }
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Sends a libpcap/tcpdump capture file to the net.");
            Console.WriteLine("Usage:");
            Console.WriteLine("\t" + Environment.GetCommandLineArgs()[0] + " <filename> [s]");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("\tfilename: the name of the dump file that will be sent to the network");
            Console.WriteLine(
                "\ts: if present, forces the packets to be sent synchronously, i.e. respecting the timestamps in the dump file. This option will work only under Windows NTx");
        }
    }
}
