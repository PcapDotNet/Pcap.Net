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

            StartReadingFromDevice(devices[index]);
        }

        private static void StartReadingFromDevice(PcapLiveDevice device)
        {
            PcapDeviceHandler handler1 = device.Open();
            PcapDeviceHandler handler2 = device.Open();
            handler1.SetFilter("port 80");
            handler2.SetFilter("port 53");
            PcapDumpFile dumpFile1 = handler1.OpenDump(@"c:\tmp.pcap");
            for (int i = 0; i != 100; ++i)
            {
                Packet packet;
                handler1.GetNextPacket(out packet);
                if (packet != null)
                {
                    System.Console.WriteLine("Port 80 Packet Length = " + packet.Length + " Timestamp = " +
                                             packet.Timestamp);
                    dumpFile1.Dump(packet);
                }
                handler2.GetNextPacket(out packet);
                if (packet != null)
                    System.Console.WriteLine("Port 53 Packet Length = " + packet.Length + " Timestamp = " + packet.Timestamp);
            }
        }
    }
}
