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
    public class DnsLayer : SimpleLayer, IEquatable<DnsLayer>
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

        public List<DnsQueryResourceRecord> Queries { get; set; }

        public List<DnsDataResourceRecord> Answers { get; set; }

        public List<DnsDataResourceRecord> Authorities { get; set; }

        public List<DnsDataResourceRecord> Additionals { get; set; }

        public IEnumerable<DnsResourceRecord> Records
        {
            get
            {
                IEnumerable<DnsResourceRecord> allRecords = null;
                foreach (IEnumerable<DnsResourceRecord> records in new IEnumerable<DnsResourceRecord>[] {Queries,Answers,Authorities,Additionals})
                {
                    if (records != null)
                        allRecords = allRecords == null ? records : allRecords.Concat(records);
                }
                return allRecords;
            }
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
        protected override void Write(byte[] buffer, int offset)
        {
            DnsDatagram.Write(buffer, offset,
                              Id, IsResponse, Opcode, IsAuthoritiveAnswer, IsTruncated, IsRecusionDesired, IsRecusionAvailable, FutureUse, ResponseCode,
                              Queries, Answers, Authorities, Additionals, DomainNameCompressionMode);
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
            return other != null &&
                   Id == other.Id &&
                   IsQuery == other.IsQuery &&
                   Opcode == other.Opcode &&
                   IsAuthoritiveAnswer == other.IsAuthoritiveAnswer &&
                   IsTruncated == other.IsTruncated &&
                   IsRecusionDesired == other.IsRecusionDesired &&
                   IsRecusionAvailable == other.IsRecusionAvailable &&
                   FutureUse == other.FutureUse &&
                   ResponseCode == other.ResponseCode &&
                   (Queries.IsNullOrEmpty() && other.Queries.IsNullOrEmpty() || Queries.SequenceEqual(other.Queries)) &&
                   (Answers.IsNullOrEmpty() && other.Answers.IsNullOrEmpty() || Answers.SequenceEqual(other.Answers)) &&
                   (Authorities.IsNullOrEmpty() && other.Authorities.IsNullOrEmpty() || Authorities.SequenceEqual(other.Authorities)) &&
                   (Additionals.IsNullOrEmpty() && other.Additionals.IsNullOrEmpty() || Additionals.SequenceEqual(other.Additionals));
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