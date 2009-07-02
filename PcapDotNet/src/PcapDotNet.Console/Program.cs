using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using PcapDotNet;
using BPacket;
using PcapDotNet.Core;

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
            IList<LivePacketDevice> devices = LivePacketDevice.AllLocalMachine;
            if (devices.Count == 0)
                return;

            for (int i = 0; i != devices.Count; ++i)
            {
                LivePacketDevice device = devices[i];
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

            IPacketDevice chosenDevice = devices[index];

            System.Console.WriteLine("Start GetNextPackets");
            GetNextPackets(chosenDevice, filter);
            System.Console.WriteLine("Finished GetNextPackets");
            System.Console.ReadKey();

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

        private static void GetNextPackets(IPacketDevice device, string filter)
        {
            using (PacketCommunicator deviceHandler = device.Open(PacketDevice.DefaultSnapshotLength, PacketDeviceOpenFlags.Promiscuous, 10 * 1000))
            {
                System.Console.WriteLine("datalink = " + deviceHandler.DataLink.Name + " description = " + deviceHandler.DataLink.Description);
                //foreach (PcapDataLink datalink in deviceHandler.SupportedDataLinks)
                  //  System.Console.WriteLine("supported datalink = " + datalink);
                deviceHandler.SetFilter(filter);
                int numPacketsGot;
                deviceHandler.GetSomePackets(1000,
                                             packet =>
                                             System.Console.WriteLine("Got packet with " + packet.Timestamp +
                                                                      " timestamp and " + packet.Length + " size"),
                                             out numPacketsGot);

                PacketTotalStatistics statistics = deviceHandler.TotalStatistics;
                System.Console.WriteLine(statistics.ToString());
            }
        }

        private static void StatisticsMode(IPacketDevice device, string filter)
        {
            using (PacketCommunicator deviceHandler = device.Open())
            {
                deviceHandler.SetFilter(filter);
                deviceHandler.Mode = DeviceHandlerMode.Statistics;

                for (int i = 0; i != 10; ++i)
                {
                    PacketSampleStatistics statistics;
                    DeviceHandlerResult result = deviceHandler.GetNextStatistics(out statistics);
                    switch (result)
                    {
                        case DeviceHandlerResult.Ok:
                            break;
                        case DeviceHandlerResult.Timeout:
                            continue;
                        case DeviceHandlerResult.Eof:
                            continue;
                    }

                    System.Console.WriteLine("Got statistics: " + statistics);
                }
            }
        }

        private static void TransmitPacketsFromFile(string filename, IPacketDevice liveDevice)
        {
            IPacketDevice offlineDevice = new OfflinePacketDevice(filename);
            using (PacketSendQueue sendQueue = new PacketSendQueue(1024 * 1024))
            {
                using (PacketCommunicator offlineHandler = offlineDevice.Open())
                {
                    for (int i = 0; i != 100; ++i)
                    {
                        Packet packet;
                        DeviceHandlerResult result = offlineHandler.GetPacket(out packet);
                        switch (result)
                        {
                            case DeviceHandlerResult.Ok:
                                break;
                            case DeviceHandlerResult.Timeout:
                                continue;
                            case DeviceHandlerResult.Eof:
                                continue;
                        }

                        sendQueue.Enqueue(packet);
                    }
                }

                using (PacketCommunicator liveHandler = liveDevice.Open())
                {
                    sendQueue.Transmit(liveHandler, true);
                }
            }
        }

        private static void SendPacketsFromFile(string filename, IPacketDevice liveDevice)
        {
            IPacketDevice offlineDevice = new OfflinePacketDevice(filename);
            using (PacketCommunicator liveHandler = liveDevice.Open())
            {
                using (PacketCommunicator offlineHandler = offlineDevice.Open())
                {
                    for (int i = 0; i != 100; ++i)
                    {
                        Packet packet;
                        DeviceHandlerResult result = offlineHandler.GetPacket(out packet);
                        switch (result)
                        {
                            case DeviceHandlerResult.Ok:
                                break;
                            case DeviceHandlerResult.Timeout:
                                continue;
                            case DeviceHandlerResult.Eof:
                                continue;
                        }

                        liveHandler.SendPacket(packet);
                    }
                }
            }
        }

        private static void CapturePacketsToFile(IPacketDevice device, string filter, string filename)
        {
            using (PacketCommunicator liveHandler = device.Open())
            {
                liveHandler.SetFilter(filter);
                PacketDumpFile dumpFile = liveHandler.OpenDump(filename);
                for (int i = 0; i != 100; ++i)
                {
                    Packet packet;
                    DeviceHandlerResult result = liveHandler.GetPacket(out packet);
                    switch (result)
                    {
                        case DeviceHandlerResult.Ok:
                            break;
                        case DeviceHandlerResult.Timeout:
                            continue;
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
