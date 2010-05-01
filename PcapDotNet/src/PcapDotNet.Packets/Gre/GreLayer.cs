using System;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Gre
{
    /// <summary>
    /// Represents a GRE layer.
    /// <seealso cref="GreDatagram"/>
    /// </summary>
    public class GreLayer : Layer, IIpV4NextLayer, IEquatable<GreLayer>
    {
        /// <summary>
        /// The GRE Version Number.
        /// </summary>
        public GreVersion Version { get; set; }

        /// <summary>
        /// The Protocol Type field contains the protocol type of the payload packet. 
        /// These Protocol Types are defined in [RFC1700] as "ETHER TYPES" and in [ETYPES]. 
        /// An implementation receiving a packet containing a Protocol Type which is not listed in [RFC1700] or [ETYPES] SHOULD discard the packet.
        /// </summary>
        public EthernetType ProtocolType { get; set; }

        /// <summary>
        /// Recursion control contains a three bit unsigned integer which contains the number of additional encapsulations which are permissible.  
        /// This SHOULD default to zero.
        /// </summary>
        public byte RecursionControl { get; set; }

        /// <summary>
        /// Must be set to zero (0).
        /// </summary>
        public byte FutureUseBits { get; set; }

        /// <summary>
        /// If the Checksum Present bit is set to 1, then the Checksum field is present and contains valid information.
        /// If either the Checksum Present bit or the Routing Present bit are set, BOTH the Checksum and Offset fields are present in the GRE packet.
        /// </summary>
        public bool ChecksumPresent { get; set; }

        /// <summary>
        /// The Checksum field contains the IP (one's complement) checksum sum of the all the 16 bit words in the GRE header and the payload packet.
        /// For purposes of computing the checksum, the value of the checksum field is zero. 
        /// This field is present only if the Checksum Present bit is set to one.
        /// In order to calculate the Checksum automatically, leave null in this field and set the ChecksumPresent field to true.
        /// </summary>
        public ushort? Checksum { get; set; }

        /// <summary>
        /// The Key field contains a four octet number which was inserted by the encapsulator.  
        /// It may be used by the receiver to authenticate the source of the packet.  
        /// The Key field is only present if the Key Present field is set to 1.
        /// null iff the Key isn't present.
        /// </summary>
        public uint? Key { get; set; }

        /// <summary>
        /// (High 2 octets of Key) Size of the payload, not including the GRE header.
        /// </summary>
        public ushort? KeyPayloadLength
        {
            get { return Key == null ? null : (ushort?)((Key.Value & 0xFFFF0000) >> 16); }
        }

        /// <summary>
        /// (Low 2 octets of Key) Contains the Peer's Call ID for the session to which this packet belongs.
        /// </summary>
        public ushort? KeyCallId
        {
            get { return Key == null ? null : (ushort?)(Key.Value & 0x0000FFFF); }
        }

        /// <summary>
        /// Sets the key according to the payload length and call id.
        /// </summary>
        /// <param name="keyPayloadLength">(High 2 octets of Key) Size of the payload, not including the GRE header.</param>
        /// <param name="keyCallId">(Low 2 octets of Key) Contains the Peer's Call ID for the session to which this packet belongs.</param>
        public void SetKey(ushort keyPayloadLength, ushort keyCallId)
        {
            Key = (uint)((keyPayloadLength << 16) | keyCallId);
        }

        /// <summary>
        /// The Sequence Number field contains an unsigned 32 bit integer which is inserted by the encapsulator.  
        /// It may be used by the receiver to establish the order in which packets have been transmitted from the encapsulator to the receiver.  
        /// null off the sequence number present bit is 0.
        /// </summary>
        public uint? SequenceNumber { get; set; }

        /// <summary>
        /// Contains the sequence number of the highest numbered GRE packet received by the sending peer for this user session.
        /// Present if A bit (Bit 8) is one (1).
        /// null iff not present.
        /// </summary>
        public uint? AcknowledgmentSequenceNumber { get; set; }

        /// <summary>
        /// The offset field indicates the octet offset from the start of the Routing field to the first octet of the active Source Route Entry to be examined.  
        /// This field is present if the Routing Present or the Checksum Present bit is set to 1, and contains valid information only if the Routing Present bit is set to 1.
        /// Should be null iff the Routing is null (routing is not present).
        /// </summary>
        public ushort? RoutingOffset { get; set; }

        /// <summary>
        /// The Routing field is optional and is present only if the Routing Present bit is set to 1.
        /// The Routing field is a list of Source Route Entries (SREs). 
        /// null iff the routing isn't present.
        /// </summary>
        public ReadOnlyCollection<GreSourceRouteEntry> Routing { get; set; }

        /// <summary>
        /// If the source route is incomplete, then the Strict Source Route bit is checked.  
        /// If the source route is a strict source route and the next IP destination or autonomous system is NOT an adjacent system, the packet MUST be dropped.
        /// </summary>
        public bool StrictSourceRoute { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get
            {
                return GreDatagram.GetHeaderLength(ChecksumPresent, Key != null, SequenceNumber != null, AcknowledgmentSequenceNumber != null, Routing);
            }
        }

        /// <summary>
        /// Writes the layer to the buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        /// <param name="payloadLength">The length of the layer's payload (the number of bytes after the layer in the packet).</param>
        /// <param name="previousLayer">The layer that comes before this layer. null if this is the first layer.</param>
        /// <param name="nextLayer">The layer that comes after this layer. null if this is the last layer.</param>
        public override void Write(byte[] buffer, int offset, int payloadLength, ILayer previousLayer, ILayer nextLayer)
        {
            EthernetType protocolType = ProtocolType;
            if (protocolType == EthernetType.None)
            {
                if (nextLayer == null)
                    throw new ArgumentException("Can't determine protocol type automatically from next layer because there is not next layer");
                IEthernetNextLayer ethernetNextLayer = nextLayer as IEthernetNextLayer;
                if (ethernetNextLayer == null)
                    throw new ArgumentException("Can't determine protocol type automatically from next layer (" + nextLayer.GetType() + ")");
                protocolType = ethernetNextLayer.PreviousLayerEtherType;
            }

            GreDatagram.WriteHeader(buffer, offset, RecursionControl, FutureUseBits, Version, protocolType, ChecksumPresent, Key, SequenceNumber, AcknowledgmentSequenceNumber, Routing, RoutingOffset, StrictSourceRoute);
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
            if (ChecksumPresent)
                GreDatagram.WriteChecksum(buffer, offset, Length + payloadLength, Checksum);
        }

        /// <summary>
        /// True iff the two objects are equal Layers.
        /// </summary>
        public bool Equals(GreLayer other)
        {
            return other != null &&
                   Version.Equals(other.Version) &&
                   ProtocolType.Equals(other.ProtocolType) &&
                   RecursionControl.Equals(other.RecursionControl) &&
                   FutureUseBits.Equals(other.FutureUseBits) &&
                   ChecksumPresent.Equals(other.ChecksumPresent) &&
                   (Checksum == null ? other.Checksum == null : Checksum.Equals(other.Checksum)) &&
                   (Key == null ? other.Key == null : Key.Equals(other.Key)) &&
                   (SequenceNumber == null ? other.SequenceNumber == null : SequenceNumber.Equals(other.SequenceNumber)) &&
                   (AcknowledgmentSequenceNumber == null ? other.AcknowledgmentSequenceNumber == null : AcknowledgmentSequenceNumber.Equals(other.AcknowledgmentSequenceNumber)) &&
                   (RoutingOffset == null ? other.RoutingOffset == null : RoutingOffset.Equals(other.RoutingOffset)) &&
                   (Routing == null ? other.Routing == null : Routing.SequenceEqual(other.Routing)) &&
                   StrictSourceRoute.Equals(other.StrictSourceRoute);
        }

        /// <summary>
        /// True iff the two objects are equal Layers.
        /// </summary>
        public override bool Equals(Layer other)
        {
            return Equals(other as GreLayer);
        }

        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// This is GRE.
        /// </summary>
        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.Gre; }
        }
    }
}