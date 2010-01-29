using System;
using System.Net.NetworkInformation;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Core.Extensions
{
    public static class LivePacketDeviceExtensions
    {
        public static NetworkInterface GetNetworkInterface(this LivePacketDevice livePacketDevice)
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (@"rpcap://\Device\NPF_" + networkInterface.Id == livePacketDevice.Name)
                    return networkInterface;
            }

            return null;
        }

        public static MacAddress GetMacAddress(this LivePacketDevice livePacketDevice)
        {
            NetworkInterface networkInterface = livePacketDevice.GetNetworkInterface();
            if (networkInterface == null)
                throw new InvalidOperationException("Couldn't find a network interface for give device");

            byte[] addressBytes = networkInterface.GetPhysicalAddress().GetAddressBytes();
            return new MacAddress(addressBytes.ReadUInt48(0, Endianity.Big));
        }
    }
}