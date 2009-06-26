using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet;
using BPacket;

namespace WinPcapDotNet.Console
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            System.Console.WriteLine("Start");
            List<PcapLiveDevice> devices = PcapLiveDevice.AllLocalMachine;
            if (devices.Count == 0)
                return;

            for (int i = 0; i != devices.Count; ++i)
            {
                PcapLiveDevice device = devices[i];
                System.Console.WriteLine(i+1 + ". " + device.Name);
            }
            int index;
            do
            {
                ConsoleKeyInfo key = System.Console.ReadKey();
                index = (key.KeyChar - '1');
                
            } while (index < 0 || index >= devices.Count);

            const string filename = @"c:\tmp.pcap";
            const string filter = "port 80";

            IPcapDevice chosenDevice = devices[index];

            System.Console.WriteLine("Start Statistics Mode");
            StatisticsMode(chosenDevice, filter);
            System.Console.WriteLine("Finished Statistics Mode");
            System.Console.ReadKey();

            System.Console.WriteLine("Start Capturing packets");
            CapturePacketsToFile(chosenDevice, filter, filename);
            System.Console.WriteLine("Finished capturing packets");
            System.Console.ReadKey();
            
            System.Console.WriteLine("Start Sending packets");
            SendPacketsFromFile(filename, chosenDevice);
            System.Console.WriteLine("Finished Sending packets");
            System.Console.ReadKey();

            System.Console.WriteLine("Start Transmitting packets");
            TransmitPacketsFromFile(filename, chosenDevice);
            System.Console.WriteLine("Finished Transmitting packets");
        }

        private static void StatisticsMode(IPcapDevice device, string filter)
        {
            using (PcapDeviceHandler deviceHandler = device.Open())
            {
                deviceHandler.SetFilter(filter);
                deviceHandler.Mode = DeviceHandlerMode.Statistics;

                for (int i = 0; i != 100; ++i)
                {
                    PcapStatistics statistics;
                    DeviceHandlerResult result = deviceHandler.GetNextStatistics(out statistics);
                    switch (result)
                    {
                        case DeviceHandlerResult.Ok:
                            break;
                        case DeviceHandlerResult.Timeout:
                            continue;
                        case DeviceHandlerResult.Error:
                            throw new InvalidOperationException("Failed reading from device");
                        case DeviceHandlerResult.Eof:
                            continue;
                    }

                    System.Console.WriteLine("Got statistics: " + statistics);
                }
            }
        }

        private static void TransmitPacketsFromFile(string filename, IPcapDevice liveDevice)
        {
            IPcapDevice offlineDevice = new PcapOfflineDevice(filename);
            using (PcapSendQueue sendQueue = new PcapSendQueue(1024 * 1024))
            {
                using (PcapDeviceHandler offlineHandler = offlineDevice.Open())
                {
                    for (int i = 0; i != 100; ++i)
                    {
                        Packet packet;
                        DeviceHandlerResult result = offlineHandler.GetNextPacket(out packet);
                        switch (result)
                        {
                            case DeviceHandlerResult.Ok:
                                break;
                            case DeviceHandlerResult.Timeout:
                                continue;
                            case DeviceHandlerResult.Error:
                                throw new InvalidOperationException("Failed reading from device");
                            case DeviceHandlerResult.Eof:
                                continue;
                        }

                        sendQueue.Enqueue(packet);
                    }
                }

                using (PcapDeviceHandler liveHandler = liveDevice.Open())
                {
                    sendQueue.Transmit(liveHandler, true);
                }
            }
        }

        private static void SendPacketsFromFile(string filename, IPcapDevice liveDevice)
        {
            IPcapDevice offlineDevice = new PcapOfflineDevice(filename);
            using (PcapDeviceHandler liveHandler = liveDevice.Open())
            {
                using (PcapDeviceHandler offlineHandler = offlineDevice.Open())
                {
                    for (int i = 0; i != 100; ++i)
                    {
                        Packet packet;
                        DeviceHandlerResult result = offlineHandler.GetNextPacket(out packet);
                        switch (result)
                        {
                            case DeviceHandlerResult.Ok:
                                break;
                            case DeviceHandlerResult.Timeout:
                                continue;
                            case DeviceHandlerResult.Error:
                                throw new InvalidOperationException("Failed reading from device");
                            case DeviceHandlerResult.Eof:
                                continue;
                        }

                        liveHandler.SendPacket(packet);
                    }
                }
            }
        }

        private static void CapturePacketsToFile(IPcapDevice device, string filter, string filename)
        {
            using (PcapDeviceHandler liveHandler = device.Open())
            {
                liveHandler.SetFilter(filter);
                PcapDumpFile dumpFile = liveHandler.OpenDump(filename);
                for (int i = 0; i != 100; ++i)
                {
                    Packet packet;
                    DeviceHandlerResult result = liveHandler.GetNextPacket(out packet);
                    switch (result)
                    {
                        case DeviceHandlerResult.Ok:
                            break;
                        case DeviceHandlerResult.Timeout:
                            continue;
                        case DeviceHandlerResult.Error:
                            throw new InvalidOperationException("Failed reading from device");
                        case DeviceHandlerResult.Eof:
                            continue;
                    }

                    System.Console.WriteLine(filter + " Packet Length = " + packet.Length + " Timestamp = " +
                                             packet.Timestamp);

                    dumpFile.Dump(packet);
                }
            }
        }
    }
}
