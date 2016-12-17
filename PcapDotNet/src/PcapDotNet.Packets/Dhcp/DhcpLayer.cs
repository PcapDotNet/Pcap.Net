using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Base;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Dhcp.Options;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp
{
    /// <summary>
    /// Represents a DHCP layer.
    /// <seealso cref="DhcpDatagram"/>
    /// </summary>
    public sealed class DhcpLayer : SimpleLayer, IEquatable<DhcpLayer>
    {
        /// <summary>
        /// Message op code
        /// </summary>
        public DhcpMessageType MessageType { get; set; }

        /// <summary>
        /// Hardware address type
        /// </summary>
        public ArpHardwareType HardwareType { get; set; }

        /// <summary>
        /// Hardware address length
        /// </summary>
        public byte HardwareAddressLength { get; set; }

        /// <summary>
        /// Client sets to zero, optionally used by relay agents when booting via a relay agent.
        /// </summary>
        public byte Hops { get; set; }

        /// <summary>
        /// Transaction ID, a random number chosen by the client, used by the client and server to associate messages and responses between a client and a server.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Filled in by client, seconds elapsed since client began address acquisition or renewal process.
        /// </summary>
        public ushort SecondsElapsed { get; set; }

        /// <summary>
        /// Flags
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        public DhcpFlags DhcpFlags { get; set; }

        /// <summary>
        /// Client IP address; only filled in if client is in BOUND, RENEW or REBINDING state and can respond to ARP requests.
        /// </summary>
        public IpV4Address ClientIpAddress { get; set; }

        /// <summary>
        /// 'your' (client) IP address.
        /// </summary>
        public IpV4Address YourClientIpAddress { get; set; }

        /// <summary>
        /// IP address of next server to use in bootstrap; returned in DHCPOFFER, DHCPACK by server.
        /// </summary>
        public IpV4Address NextServerIpAddress { get; set; }

        /// <summary>
        /// Relay agent IP address, used in booting via a relay agent.
        /// </summary>
        public IpV4Address RelayAgentIpAddress { get; set; }

        /// <summary>
        /// Client hardware address.
        /// </summary>
        public DataSegment ClientHardwareAddress { get; set; }

        /// <summary>
        /// Client MAC address
        /// </summary>
        public MacAddress ClientMacAddress
        {
            get
            {
                if (ClientHardwareAddress == null)
                    return MacAddress.Zero;
                return new MacAddress(ClientHardwareAddress.ReadUInt48(0, Endianity.Big));
            }
            set
            {
                if (ClientHardwareAddress == null)
                    ClientHardwareAddress = new DataSegment(new byte[16]);
                ClientHardwareAddress.Buffer.Write(0, value, Endianity.Big);
            }
        }

        /// <summary>
        /// Optional server host name
        /// </summary>
        public string ServerHostName { get; set; }

        /// <summary>
        /// Boot file name; "generic" name or null in DHCPDISCOVER, fully qualified directory-path name in DHCPOFFER.
        /// </summary>
        public string BootFileName { get; set; }

        /// <summary>
        /// true if the magic dhcp-cookie is set
        /// </summary>
        public bool IsDhcp { get; set; }

        /// <summary>
        /// Optional parameters field
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<DhcpOption> Options { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get
            {
                return DhcpDatagram.GetLength(IsDhcp, Options);
            }
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        protected override void Write(byte[] buffer, int offset)
        {
            DhcpDatagram.Write(buffer, offset,
                              MessageType, HardwareType, HardwareAddressLength, Hops, TransactionId, SecondsElapsed, DhcpFlags, ClientIpAddress, YourClientIpAddress,
                              NextServerIpAddress, RelayAgentIpAddress, ClientHardwareAddress, ServerHostName, BootFileName, IsDhcp, Options);
        }

        /// <summary>
        /// Finalizes the layer data in the buffer.
        /// Used for fields that must be calculated according to the layer's payload (like checksum).
        /// </summary>
        /// <param name="buffer">The buffer to finalize the layer in.</param>
        /// <param name="offset">The offset in the buffer the layer starts.</param>
        /// <param name="payloadLength">The length of the layer's payload (the number of bytes after the layer in the packet).</param>
        /// <param name="nextLayer">The layer that comes after this layer. null if this is the last layer.</param>
        public override void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
        }

        /// <summary>
        /// True if the two objects are equal Layers.
        /// </summary>
        public bool Equals(DhcpLayer other)
        {
            return other != null &&
                   Equals(MessageType, other.MessageType) &&
                   Equals(HardwareType, other.HardwareType) &&
                   Equals(HardwareAddressLength, other.HardwareAddressLength) &&
                   Equals(Hops, other.Hops) &&
                   Equals(TransactionId, other.TransactionId) &&
                   Equals(SecondsElapsed, other.SecondsElapsed) &&
                   Equals(DhcpFlags, other.DhcpFlags) &&
                   Equals(ClientIpAddress, other.ClientIpAddress) &&
                   Equals(YourClientIpAddress, other.YourClientIpAddress) &&
                   Equals(NextServerIpAddress, other.NextServerIpAddress) &&
                   Equals(RelayAgentIpAddress, other.RelayAgentIpAddress) &&
                   Equals(ClientHardwareAddress, ClientHardwareAddress) &&
                   Equals(ServerHostName, other.ServerHostName) &&
                   Equals(BootFileName, other.BootFileName) &&
                   Equals(IsDhcp, other.IsDhcp) &&
                   (Options.IsNullOrEmpty() && other.Options.IsNullOrEmpty() || Options.SequenceEqual(other.Options));
        }

        /// <summary>
        /// True if the two objects are equal Layers.
        /// </summary>
        public override bool Equals(Layer other)
        {
            return Equals(other as DhcpLayer);
        }
    }
}