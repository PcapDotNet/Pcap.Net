using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// Contains the common part of UDP and TCP layers.
    /// <seealso cref="TransportDatagram"/>
    /// </summary>
    public abstract class TransportLayer : Layer, IIpNextTransportLayer, IEquatable<TransportLayer>
    {
        /// <summary>
        /// Checksum is the 16-bit one's complement of the one's complement sum of a pseudo header of information from the IP header, 
        /// the Transport header, and the data, padded with zero octets at the end (if necessary) to make a multiple of two octets.
        /// </summary>
        public ushort? Checksum { get; set; }

        /// <summary>
        /// Indicates the port of the sending process.
        /// In UDP, this field is optional and may only be assumed to be the port 
        /// to which a reply should be addressed in the absence of any other information.
        /// If not used in UDP, a value of zero is inserted.
        /// </summary>
        public ushort SourcePort { get; set; }

        /// <summary>
        /// Destination Port has a meaning within the context of a particular internet destination address.
        /// </summary>
        public ushort DestinationPort { get; set; }

        /// <summary>
        /// The protocol that should be written in the previous (IPv4) layer.
        /// </summary>
        public abstract IpV4Protocol PreviousLayerProtocol { get; }

        /// <summary>
        /// Whether the checksum should be calculated.
        /// Can be false in UDP because the checksum is optional. false means that the checksum will be left zero.
        /// </summary>
        public virtual bool CalculateChecksum
        {
            get { return true; }
        }

        /// <summary>
        /// The offset in the layer where the checksum should be written.
        /// </summary>
        public abstract int ChecksumOffset { get; }

        /// <summary>
        /// Whether the checksum is optional in the layer.
        /// </summary>
        public abstract bool IsChecksumOptional { get; }

        /// <summary>
        /// Two Transport layers are equal if they have they have the same previous layer protocol value, checksum, source and destination ports, 
        /// and if the specific transport protocol fields are equal.
        /// </summary>
        public bool Equals(TransportLayer other)
        {
            return other != null &&
                   PreviousLayerProtocol == other.PreviousLayerProtocol &&
                   Checksum == other.Checksum &&
                   SourcePort == other.SourcePort && DestinationPort == other.DestinationPort &&
                   EqualFields(other);
        }

        /// <summary>
        /// Two Transport layers are equal if they have they have the same previous layer protocol value, checksum, source and destination ports, 
        /// and if the specific transport protocol fields are equal.
        /// </summary>
        public sealed override bool Equals(Layer other)
        {
            return Equals(other as TransportLayer);
        }

        /// <summary>
        /// Returns a hash code for the layer.
        /// The hash code is a XOR of the combination of the source and destination ports and the hash codes of the layer length, data link and checksum.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Checksum.GetHashCode() ^ BitSequence.Merge(SourcePort, DestinationPort).GetHashCode();
        }

        /// <summary>
        /// True iff the specific transport layer fields are equal.
        /// </summary>
        protected virtual bool EqualFields(TransportLayer other)
        {
            return true;
        }
    }
}