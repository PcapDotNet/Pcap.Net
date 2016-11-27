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
        public DhcpMessageType MessageType { get; set; }

        public ArpHardwareType HardwareType { get; set; }

        public byte HardwareAddressLength { get; set; }

        public byte Hops { get; set; }

        public int TransactionId { get; set; }

        public ushort SecondsElapsed { get; set; }

        public DhcpFlags Flags { get; set; }

        public IpV4Address ClientIpAddress { get; set; }

        public IpV4Address YourClientIpAddress { get; set; }

        public IpV4Address NextServerIpAddress { get; set; }

        public IpV4Address RelayAgentIpAddress { get; set; }

        public DataSegment ClientHardwareAddress { get; set; }

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

        public string ServerHostName { get; set; }

        public string BootFileName { get; set; }

        public bool IsDhcp { get; set; }

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
                              MessageType, HardwareType, HardwareAddressLength, Hops, TransactionId, SecondsElapsed, Flags, ClientIpAddress, YourClientIpAddress,
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
                   Equals(Flags, other.Flags) &&
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