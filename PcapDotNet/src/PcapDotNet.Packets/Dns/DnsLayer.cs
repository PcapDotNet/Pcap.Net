using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// Represents a DNS layer.
    /// <seealso cref="DnsDatagram"/>
    /// </summary>
    public class DnsLayer : Layer, IEquatable<DnsLayer>
    {

        public ushort Id { get; set; }
        public bool IsResponse { get; set; }
        public bool IsQuery
        {
            get { return !IsResponse; } 
            set { IsResponse = !value; }
        }

        public DnsOpcode Opcode{ get; set; }

        public bool IsAuthoritiveAnswer { get; set; }

        public bool IsTruncated { get; set; }

        public bool IsRecusionDesired { get; set; }

        public bool IsRecusionAvailable { get; set;}

        public byte FutureUse { get; set; }

        public DnsResponseCode ResponseCode { get; set; }

        public List<DnsQueryResouceRecord> Queries { get; set; }

        public List<DnsDataResourceRecord> Answers { get; set; }

        public List<DnsDataResourceRecord> Authorities { get; set; }

        public List<DnsDataResourceRecord> Additionals { get; set; }

        public IEnumerable<DnsResourceRecord> Records
        {
            get { return Queries.Concat<DnsResourceRecord>(Answers).Concat(Authorities).Concat(Additionals); }
        }

        public DnsDomainNameCompressionMode DomainNameCompressionMode { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get
            {
                return DnsDatagram.GetLength(Records, DomainNameCompressionMode);
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
        /// True iff the two objects are equal Layers.
        /// </summary>
        public bool Equals(DnsLayer other)
        {
            return other != null;
        }

        /// <summary>
        /// True iff the two objects are equal Layers.
        /// </summary>
        public override bool Equals(Layer other)
        {
            return Equals(other as DnsLayer);
        }
    }
}