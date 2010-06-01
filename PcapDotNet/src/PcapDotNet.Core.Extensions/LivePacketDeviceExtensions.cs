using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Core.Extensions
{
    /// <summary>
    /// Extension methods for LivePacketDevice class.
    /// </summary>
    public static class LivePacketDeviceExtensions
    {
        const string NamePrefix = @"rpcap://\Device\NPF_";
        const string NetworkConnectionConfigKey = @"SYSTEM\CurrentControlSet\Control\Network\{4D36E972-E325-11CE-BFC1-08002BE10318}";

        /// <summary>
        /// Returns the GUID (NetCfgInstanceId) for a <see cref="LivePacketDevice"/> instance.
        /// The GUID is parsed from the <see cref="LivePacketDevice.Name"/> property.
        /// </summary>
        /// <param name="livePacketDevice">The <see cref="LivePacketDevice"/> instance.</param>
        /// <returns>The GUID (NetCfgInstanceId) of the <see cref="LivePacketDevice"/> instance.</returns>
        /// <exception cref="InvalidOperationException">When the <see cref="LivePacketDevice.Name"/> doesn't match the expectations.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static string GetGuid(this LivePacketDevice livePacketDevice)
        {
            if (livePacketDevice == null) 
                throw new ArgumentNullException("livePacketDevice");

            string livePacketDeviceName = livePacketDevice.Name;
            if (!livePacketDeviceName.StartsWith(NamePrefix, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                                                                  "Invalid Device Name format: {0} (should start with: {1})",
                                                                  livePacketDevice.Name, NamePrefix));
            }

            return livePacketDevice.Name.Substring(NamePrefix.Length);
        }

        /// <summary>
        /// Returns the PNPDeviceID for a <see cref="LivePacketDevice"/> instance.
        /// The PNPDeviceID is retrieved by querying the registry.
        /// </summary>
        /// <param name="livePacketDevice">The <see cref="LivePacketDevice"/> instance.</param>
        /// <returns>The PNPDeviceID of the <see cref="LivePacketDevice"/> instance.</returns>
        /// <exception cref="InvalidOperationException">When the PNPDeviceID cannot be retrieved from the registry.</exception>
        public static string GetPnpDeviceId(this LivePacketDevice livePacketDevice)
        {
            string guid = livePacketDevice.GetGuid();

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(NetworkConnectionConfigKey + @"\" + guid + @"\Connection"))
            {
                string pnpDeviceId = key.GetValue("PnpInstanceID") as string;
                if (pnpDeviceId == null)
                    throw new InvalidOperationException("Could not find PNP Device ID in the registry");
                return pnpDeviceId;
            }
        }

        /// <summary>
        /// Returns the network interface of the packet device.
        /// The interface is found using its id.
        /// If no interface is found, null is returned.
        /// </summary>
        /// <param name="livePacketDevice">The LivePacketDevice to look for a matching network interface for.</param>
        /// <returns>The network interface found according to the given device or null if none is found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static NetworkInterface GetNetworkInterface(this LivePacketDevice livePacketDevice)
        {
            if (livePacketDevice == null) 
                throw new ArgumentNullException("livePacketDevice");

            string guid = GetGuid(livePacketDevice);
            return NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(networkInterface => networkInterface.Id == guid);
        }

        /// <summary>
        /// Returns the <see cref="MacAddress"/> of the network interface of the given device.
        /// If no interface matches the given packet device, an exception is thrown.
        /// We first look for the device using <see cref="NetworkInterface.GetAllNetworkInterfaces"/> and if that fails we look for them using WMI.
        /// </summary>
        /// <param name="livePacketDevice">The packet device to look for the matching interface.</param>
        /// <returns>The <see cref="MacAddress"/> of the given device's matching interface.</returns>
        public static MacAddress GetMacAddress(this LivePacketDevice livePacketDevice)
        {
            // First, look for a NetworkInterface
            NetworkInterface networkInterface = livePacketDevice.GetNetworkInterface();
            if (networkInterface != null)
            {
                byte[] addressBytes = networkInterface.GetPhysicalAddress().GetAddressBytes();
                return new MacAddress(addressBytes.ReadUInt48(0, Endianity.Big));
            }

            return livePacketDevice.GetMacAddressWmi();
        }

        /// <summary>
        /// Returns the <see cref="MacAddress"/> for a <see cref="LivePacketDevice"/> instance.
        /// The <see cref="MacAddress"/> is retrieved through using WMI.
        /// </summary>
        /// <param name="livePacketDevice">The <see cref="LivePacketDevice"/> instance.</param>
        /// <returns>The <see cref="MacAddress"/> of the <see cref="LivePacketDevice"/> instance.</returns>
        /// <exception cref="InvalidOperationException">When the <see cref="MacAddress"/> cannot be retrieved using WMI.</exception>
        private static MacAddress GetMacAddressWmi(this LivePacketDevice livePacketDevice)
        {
            string pnpDeviceId = livePacketDevice.GetPnpDeviceId();
            string escapedPnpDeviceId = pnpDeviceId.Replace(@"\", @"\\");

            ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
            scope.Connect();

            var searcher = new ManagementObjectSearcher(scope, new SelectQuery("SELECT MACAddress FROM Win32_NetworkAdapter WHERE PNPDeviceID='" + escapedPnpDeviceId + "'"));
            foreach (ManagementObject managementObject in searcher.Get())
            {
                string macAddress = managementObject["MACAddress"] as string;
                if (string.IsNullOrEmpty(macAddress))
                    throw new InvalidOperationException("No MAC Address for WMI instance with PNP Device ID: " + pnpDeviceId);

                return new MacAddress(macAddress);
            }

            throw new InvalidOperationException("No MAC Address for WMI instance with PNP Device ID: " + pnpDeviceId);
        }
    }
}