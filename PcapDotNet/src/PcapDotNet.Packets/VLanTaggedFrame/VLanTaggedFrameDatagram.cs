using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.VLanTaggedFrame
{
    /// <summary>
    /// IEEE 802.1Q.
    /// <pre>
    /// +-----+-----+-----+------+------------------+
    /// | bit | 0-2 | 3   | 4-15 | 16-31            |
    /// +-----+-----+-----+------+------------------+
    /// | 0   |	TCI              | EtherType/Length |
    /// +-----+-----+-----+------+------------------+
    /// | 0   |	PCP | CFI | VID  | EtherType/Length |
    /// +-----+-----+-----+------+------------------+
    /// </pre>
    /// </summary>
    public sealed class VLanTaggedFrameDatagram : EthernetBaseDatagram
    {
        private static class Offset
        {
            public const int PriorityCodePoint = 0;
            public const int CanonicalFormatIndicator = PriorityCodePoint;
            public const int VLanIdentifier = CanonicalFormatIndicator;
            public const int EtherTypeLength = VLanIdentifier + sizeof(ushort);
        }

        /// <summary>
        /// The number of bytes in the header takes.
        /// </summary>
        public const int HeaderLengthValue = Offset.EtherTypeLength + sizeof(ushort);

        private static class Mask
        {
            public const byte PriorityCodePoint = 0xE0;
            public const byte CanonicalFormatIndicator = 0x10;
            public const ushort VLanIdentifier = 0x0FFF;
        }

        private static class Shift
        {
            public const int PriorityCodePoint = 5;
        }

        /// <summary>
        /// The null VLAN ID.
        /// Indicates that the tag header contains only priority information; no VLAN identifier is present in the frame.
        /// This VID value shall not be configured as a PVID or a member of a VID Set, or configured in any Filtering Database entry, 
        /// or used in any Management operation.
        /// </summary>
        public const ushort NullVLanIdentifier = 0x000;

        /// <summary>
        /// The default PVID value used for classifying frames on ingress through a Bridge Port.
        /// The PVID value of a Port can be changed by management.
        /// </summary>
        public const ushort DefaultPortVLanIdentifier = 0x001;

        /// <summary>
        /// Reserved for implementation use.
        /// This VID value shall not be configured as a PVID or a member of a VID Set, or transmitted in a tag header.
        /// This VID value may be used to indicate a wildcard match for the VID in management operations or Filtering Database entries.
        /// </summary>
        public const ushort MaxVLanIdentifier = 0xFFF;

        /// <summary>
        /// Header length in bytes.
        /// </summary>
        public override int HeaderLength
        {
            get { return HeaderLengthValue; }
        }

        /// <summary>
        /// Indicates the frame priority level.
        /// Values are from 0 (best effort) to 7 (highest); 1 represents the lowest priority.
        /// These values can be used to prioritize different classes of traffic (voice, video, data, etc.).
        /// </summary>
        public ClassOfService PriorityCodePoint
        {
            get { return (ClassOfService)((this[Offset.PriorityCodePoint] & Mask.PriorityCodePoint) >> Shift.PriorityCodePoint); }
        }

        /// <summary>
        /// If reset, all MAC Address information that may be present in the MSDU is in Canonical format and the tag comprises solely the TPID and TCI fields,
        /// i.e., the tag does not contain an Embedded Routing Information Field (E-RIF).
        /// </summary>
        public bool CanonicalFormatIndicator
        {
            get { return ReadBool(Offset.CanonicalFormatIndicator, Mask.CanonicalFormatIndicator); }
        }

        /// <summary>
        /// A VLAN-aware Bridge may not support the full range of VID values but shall support the use of all VID values in the range 0 through a maximum N,
        /// less than or equal to 4094 and specified for that implementation.
        /// </summary>
        public ushort VLanIdentifier
        {
            get { return (ushort)(ReadUShort(Offset.VLanIdentifier, Endianity.Big) & Mask.VLanIdentifier); }
        }

        /// <summary>
        /// A combination of pcp (PriorityCodePoint), cfi (CanonicalFormatIndicator) and vid (VLanIdentifier).
        /// </summary>
        public ushort TagControlInformation
        {
            get { return CalculateTagControlInformation(PriorityCodePoint, CanonicalFormatIndicator, VLanIdentifier); }
        }

        /// <summary>
        /// Ethernet type (next protocol).
        /// </summary>
        public override EthernetType EtherType
        {
            get
            {
                return (EthernetType)ReadUShort(Offset.EtherTypeLength, Endianity.Big);
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new VLanTaggedFrameLayer
                   {
                       PriorityCodePoint = PriorityCodePoint,
                       CanonicalFormatIndicator = CanonicalFormatIndicator,
                       VLanIdentifier = VLanIdentifier,
                       EtherType = EtherType,
                   };
        }

        /// <summary>
        /// The datagram is valid if the length is correct according to the header.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (Length < HeaderLength)
                return false;

            Datagram payload = PayloadByEtherType;
            return payload == null || payload.IsValid;
        }

        internal VLanTaggedFrameDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset, ClassOfService priorityCodePoint, bool canonicalFormatIndicator, ushort vLanIdentifier, EthernetType etherType)
        {
            ushort tagControlInformation = CalculateTagControlInformation(priorityCodePoint, canonicalFormatIndicator, vLanIdentifier);
            buffer.Write(offset + Offset.PriorityCodePoint, tagControlInformation, Endianity.Big);
            buffer.Write(offset + Offset.EtherTypeLength, (ushort)etherType, Endianity.Big);
        }

        internal static ushort CalculateTagControlInformation(ClassOfService priorityCodePoint, bool canonicalFormatIndicator, ushort vLanIdentifier)
        {
            return (ushort)(((((((byte)priorityCodePoint) << Shift.PriorityCodePoint) & Mask.PriorityCodePoint) |
                              (canonicalFormatIndicator ? Mask.CanonicalFormatIndicator : 0x00)) << 8) |
                            (vLanIdentifier & Mask.VLanIdentifier));
        }
    }
}