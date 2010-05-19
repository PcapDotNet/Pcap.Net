using System;
using System.Net.NetworkInformation;
using PcapDotNet.Base;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Core.Extensions
{
    /// <summary>
    /// Extension methods for LivePacketDevice class.
    /// </summary>
    public static class LivePacketDeviceExtensions
    {
        /// <summary>
        /// Returns the network interface of the packet device.
        /// The interface is found using its id.
        /// If no interface is found, null is returned.
        /// </summary>
        /// <param name="livePacketDevice">The LivePacketDevice to look for a matching network interface for.</param>
        /// <returns>The network interface found according to the given device or null if none is found.</returns>
        public static NetworkInterface GetNetworkInterface(this LivePacketDevice livePacketDevice)
        {
            if (livePacketDevice == null) 
                throw new ArgumentNullException("livePacketDevice");

            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (@"rpcap://\Device\NPF_" + networkInterface.Id == livePacketDevice.Name)
                    return networkInterface;
            }

            return null;
        }

        /// <summary>
        /// Returns the MacAddress of the network interface of the given device.
        /// If no interface matches the given packet device, an exception is thrown.
        /// </summary>
        /// <param name="livePacketDevice">The packet device to look for the matching interface.</param>
        /// <returns>The MacAddress of the given device's matching interface.</returns>
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