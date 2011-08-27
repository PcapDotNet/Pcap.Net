using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Arp
{
    /// <summary>
    /// The following is the packet structure used for ARP requests and replies. 
    /// On Ethernet networks, these packets use an EtherType of 0x0806, and are sent to the broadcast MAC address of FF:FF:FF:FF:FF:FF. 
    /// Note that the EtherType (0x0806) is used in the Ethernet header, and should not be used as the PTYPE of the ARP packet. 
    /// The ARP type (0x0806) should never be used in the PTYPE field of an ARP packet, since a hardware protocol address should never be linked to the ARP protocol. 
    /// Note that the packet structure shown in the table has SHA and THA as 48-bit fields and SPA and TPA as 32-bit fields but this is just for convenience � 
    /// their actual lengths are determined by the hardware &amp; protocol length fields.
    /// <pre>
    /// +-----+------------------------+------------------------+-----------------------------------------------+
    /// | bit | 0-7                    | 8-15                   | 16-31                                         |
    /// +-----+------------------------+------------------------+-----------------------------------------------+
    /// | 0   |	Hardware type (HTYPE)                           | Protocol type (PTYPE)                         |
    /// +-----+------------------------+------------------------+-----------------------------------------------+
    /// | 32  | Hardware length (HLEN) | Protocol length (PLEN) | Operation (OPER)                              |
    /// +-----+------------------------+------------------------+-----------------------------------------------+
    /// | 64  | Sender hardware address (SHA) (first 32 bits)                                                   |
    /// +-----+-------------------------------------------------+-----------------------------------------------+
    /// | 96  | Sender hardware address (SHA) (last 16 bits)    | Sender protocol address (SPA) (first 16 bits) |
    /// +-----+-------------------------------------------------+-----------------------------------------------+
    /// | 128 | Sender protocol address (SPA) (last 16 bits)    | Target hardware address (THA) (first 16 bits) |
    /// +-----+-------------------------------------------------+-----------------------------------------------+
    /// | 160 |	Target hardware address (THA) (last 32 bits)                                                    |
    /// +-----+-------------------------------------------------------------------------------------------------+
    /// | 192 | Target protocol address (TPA)                                                                   |
    /// +-----+-------------------------------------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public sealed class ArpDatagram : Datagram
    {
        private static class Offset
        {
            public const int HardwareType = 0;
            public const int ProtocolType = 2;
            public const int HardwareLength = 4;
            public const int ProtocolLength = 5;
            public const int Operation = 6;
            public const int SenderHardwareAddress = 8;
        }

        /// <summary>
        /// The number of bytes in the ARP header without the addresses (that vary in size).
        /// </summary>
        public const int HeaderBaseLength = 8;

        /// <summary>
        /// The number of bytes in the ARP header.
        /// </summary>
        public int HeaderLength
        {
            get { return GetHeaderLength(HardwareLength, ProtocolLength); }
        }

        /// <summary>
        /// Each data link layer protocol is assigned a number used in this field.
        /// </summary>
        public ArpHardwareType HardwareType
        {
            get { return (ArpHardwareType)ReadUShort(Offset.HardwareType, Endianity.Big); }
        }

        /// <summary>
        /// Each protocol is assigned a number used in this field.
        /// </summary>
        public EthernetType ProtocolType
        {
            get { return (EthernetType)ReadUShort(Offset.ProtocolType, Endianity.Big); }
        }

        /// <summary>
        /// Length in bytes of a hardware address. Ethernet addresses are 6 bytes long.
        /// </summary>
        public byte HardwareLength
        {
            get { return this[Offset.HardwareLength]; }
        }

        /// <summary>
        /// Length in bytes of a logical address. IPv4 address are 4 bytes long.
        /// </summary>
        public byte ProtocolLength
        {
            get { return this[Offset.ProtocolLength]; }
        }

        /// <summary>
        /// Specifies the operation the sender is performing.
        /// </summary>
        public ArpOperation Operation
        {
            get { return (ArpOperation)ReadUShort(Offset.Operation, Endianity.Big); }
        }

        /// <summary>
        /// Hardware address of the sender.
        /// </summary>
        public ReadOnlyCollection<byte> SenderHardwareAddress
        {
            get { return ReadBytes(Offset.SenderHardwareAddress, HardwareLength).AsReadOnly(); }
        }

        /// <summary>
        /// Protocol address of the sender.
        /// </summary>
        public ReadOnlyCollection<byte> SenderProtocolAddress
        {
            get { return ReadBytes(OffsetSenderProtocolAddress, ProtocolLength).AsReadOnly(); }
        }

        /// <summary>
        /// Protocol IPv4 address of the sender.
        /// </summary>
        public IpV4Address SenderProtocolIpV4Address
        {
            get { return ReadIpV4Address(OffsetSenderProtocolAddress, Endianity.Big); }
        }

        /// <summary>
        /// Hardware address of the intended receiver. 
        /// This field is ignored in requests.
        /// </summary>
        public ReadOnlyCollection<byte> TargetHardwareAddress
        {
            get { return ReadBytes(OffsetTargetHardwareAddress, HardwareLength).AsReadOnly(); }
        }

        /// <summary>
        /// Protocol address of the intended receiver.
        /// </summary>
        public ReadOnlyCollection<byte> TargetProtocolAddress
        {
            get { return ReadBytes(OffsetTargetProtocolAddress, ProtocolLength).AsReadOnly(); }
        }

        /// <summary>
        /// Protocol IPv4 address of the intended receiver.
        /// </summary>
        public IpV4Address TargetProtocolIpV4Address
        {
            get { return ReadIpV4Address(OffsetTargetProtocolAddress, Endianity.Big); }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new ArpLayer
                   {
                       SenderHardwareAddress = SenderHardwareAddress,
                       SenderProtocolAddress = SenderProtocolAddress,
                       TargetHardwareAddress = TargetHardwareAddress,
                       TargetProtocolAddress = TargetProtocolAddress,
                       ProtocolType = ProtocolType,
                       Operation = Operation,
                   };
        }

        /// <summary>
        /// The datagram is valid if the length is correct according to the header.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return Length >= HeaderBaseLength && Length == HeaderLength;
        }

        internal static ArpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            if (length <= HeaderBaseLength)
                return new ArpDatagram(buffer, offset, length);

            int headerLength = GetHeaderLength(buffer[offset + Offset.HardwareLength], buffer[offset + Offset.ProtocolLength]);
            return new ArpDatagram(buffer, offset, Math.Min(length, headerLength));
        }

        internal static int GetHeaderLength(int hardwareLength, int protocolLength)
        {
            return HeaderBaseLength + 2 * hardwareLength + 2 * protocolLength;
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         ArpHardwareType hardwareType, EthernetType protocolType, ArpOperation operation,
                                         IList<byte> senderHardwareAddress, IList<byte> senderProtocolAddress,
                                         IList<byte> targetHardwareAddress, IList<byte> targetProtocolAddress)
        {
            buffer.Write(ref offset, (ushort)hardwareType, Endianity.Big);
            buffer.Write(ref offset, (ushort)protocolType, Endianity.Big);
            buffer.Write(ref offset, (byte)senderHardwareAddress.Count);
            buffer.Write(ref offset, (byte)senderProtocolAddress.Count);
            buffer.Write(ref offset, (ushort)operation, Endianity.Big);
            buffer.Write(ref offset, senderHardwareAddress);
            buffer.Write(ref offset, senderProtocolAddress);
            buffer.Write(ref offset, targetHardwareAddress);
            buffer.Write(ref offset, targetProtocolAddress);
        }

        private ArpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        private int OffsetSenderProtocolAddress
        {
            get { return Offset.SenderHardwareAddress + HardwareLength; }
        }

        private int OffsetTargetHardwareAddress
        {
            get { return OffsetSenderProtocolAddress + ProtocolLength; }
        }

        private int OffsetTargetProtocolAddress
        {
            get { return OffsetTargetHardwareAddress + HardwareLength; }
        }
    }
}