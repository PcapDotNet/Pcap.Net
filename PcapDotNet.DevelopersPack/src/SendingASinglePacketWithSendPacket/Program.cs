using System;
using System.Collections.Generic;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

namespace SendingASinglePacketWithSendPacket
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
            PacketDevice selectedDevice = allDevices[deviceIndex - 1];
    
            // Open the output device
            using (PacketCommunicator communicator = selectedDevice.Open(100,                                    // name of the device
                                                                         PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                                                         1000))                                  // read timeout
            {
                // Supposing to be on ethernet, set mac destination to 1:1:1:1:1:1
                MacAddress source = new MacAddress("1:1:1:1:1:1");

                // set mac source to 2:2:2:2:2:2
                MacAddress destination = new MacAddress("2:2:2:2:2:2");

                // Fill the rest of the packet (ethernet payload)
                byte[] ethernetPayloadBuffer = new byte[100];
                for (int i = 0; i != ethernetPayloadBuffer.Length; ++i)
                    ethernetPayloadBuffer[i] = (byte)(i % 256);
                Datagram ethernetPayload = new Datagram(ethernetPayloadBuffer);

                // Create the packet
                Packet packet = PacketBuilder.Ethernet(DateTime.Now, source, destination, EthernetType.IpV4, ethernetPayload);

                // Send down the packet
                communicator.SendPacket(packet);
            }
        }
    }
}
