using System;
using System.Collections.Generic;
using PcapDotNet.Core;

namespace GatheringStatisticsOnTheNetworkTraffic
{
    class Program
    {
        static void Main(string[] args)
        {
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

            // Open the output adapter
            using (
                PacketCommunicator communicator = selectedOutputDevice.Open(100, PacketDeviceOpenAttributes.Promiscuous,
                                                                            1000))
            {
                // Compile and set the filter
                communicator.SetFilter("tcp");

                // Put the interface in statstics mode
                communicator.Mode = PacketCommunicatorMode.Statistics;

                Console.WriteLine("TCP traffic summary:");

                // Start the main loop
                communicator.ReceiveStatistics(0, StatisticsHandler);
            }
        }

        private static void StatisticsHandler(PacketSampleStatistics statistics)
        {
            // Current sample time
            DateTime currentTimestamp = statistics.Timestamp;

            // Previous sample time
            DateTime previousTimestamp = _lastTimestamp;

            // Set _lastTimestamp for the next iteration
            _lastTimestamp = currentTimestamp;

            // If there wasn't a previous sample than skip this iteration (it's the first iteration)
            if (previousTimestamp == DateTime.MinValue)
                return;

            // Calculate the delay from the last sample
            double delayInSeconds = (currentTimestamp - previousTimestamp).TotalSeconds;

            // Calculate bits per second
            double bitsPerSecond = statistics.AcceptedBytes * 8 / delayInSeconds;

            // Calculate packets per second
            double packetsPerSecond = statistics.AcceptedPackets / delayInSeconds;

            // Print timestamp and samples
            Console.WriteLine(statistics.Timestamp + " BPS: " + bitsPerSecond + " PPS: " + packetsPerSecond);
        }

        private static DateTime _lastTimestamp;
    }
}
