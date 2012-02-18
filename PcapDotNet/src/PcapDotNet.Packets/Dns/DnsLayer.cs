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
    public sealed class DnsLayer : SimpleLayer, IEquatable<DnsLayer>
    {
        /// <summary>
        /// A 16 bit identifier assigned by the program that generates any kind of query.  
        /// This identifier is copied the corresponding reply and can be used by the requester to match up replies to outstanding queries.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// A one bit field that specifies whether this message is a query (0), or a response (1).
        /// </summary>
        public bool IsResponse { get; set; }

        /// <summary>
        /// Specifies whether this message is a query or a response.
        /// </summary>
        public bool IsQuery
        {
            get { return !IsResponse; } 
            set { IsResponse = !value; }
        }

        /// <summary>
        /// Specifies kind of query in this message.  
        /// This value is set by the originator of a query and copied into the response.
        /// </summary>
        public DnsOpCode OpCode { get; set; }

        /// <summary>
        /// This bit is valid in responses, and specifies that the responding name server is an authority for the domain name in question section.
        /// Note that the contents of the answer section may have multiple owner names because of aliases.  
        /// The AA bit corresponds to the name which matches the query name, or the first owner name in the answer section.
        /// </summary>
        public bool IsAuthoritativeAnswer { get; set; }

        /// <summary>
        /// Specifies that this message was truncated due to length greater than that permitted on the transmission channel.
        /// </summary>
        public bool IsTruncated { get; set; }

        /// <summary>
        /// This bit may be set in a query and is copied into the response.  
        /// If RD is set, it directs the name server to pursue the query recursively. 
        /// Recursive query support is optional.
        /// </summary>
        public bool IsRecursionDesired { get; set; }

        /// <summary>
        /// This bit is set or cleared in a response, and denotes whether recursive query support is available in the name server.
        /// </summary>
        public bool IsRecursionAvailable { get; set; }

        /// <summary>
        /// Reserved for future use.  
        /// Must be false in all queries and responses.
        /// </summary>
        public bool FutureUse { get; set; }

        /// <summary>
        /// The name server side of a security-aware recursive name server must not set the AD bit in a response
        /// unless the name server considers all RRsets in the Answer and Authority sections of the response to be authentic.
        /// The name server side should set the AD bit if and only if the resolver side considers all RRsets in the Answer section
        /// and any relevant negative response RRs in the Authority section to be authentic.
        /// The resolver side must follow the Authenticating DNS Responses procedure to determine whether the RRs in question are authentic.
        /// However, for backward compatibility, a recursive name server may set the AD bit when a response includes unsigned CNAME RRs
        /// if those CNAME RRs demonstrably could have been synthesized from an authentic DNAME RR that is also included in the response
        /// according to the synthesis rules described in RFC 2672.
        /// </summary>
        public bool IsAuthenticData { get; set; }

        /// <summary>
        /// Exists in order to allow a security-aware resolver to disable signature validation
        /// in a security-aware name server's processing of a particular query.
        /// 
        /// The name server side must copy the setting of the CD bit from a query to the corresponding response.
        /// 
        /// The name server side of a security-aware recursive name server must pass the state of the CD bit to the resolver side
        /// along with the rest of an initiating query,
        /// so that the resolver side will know whether it is required to verify the response data it returns to the name server side.
        /// If the CD bit is set, it indicates that the originating resolver is willing to perform whatever authentication its local policy requires.
        /// Thus, the resolver side of the recursive name server need not perform authentication on the RRsets in the response.
        /// When the CD bit is set, the recursive name server should, if possible, return the requested data to the originating resolver, 
        /// even if the recursive name server's local authentication policy would reject the records in question.
        /// That is, by setting the CD bit, the originating resolver has indicated that it takes responsibility for performing its own authentication,
        /// and the recursive name server should not interfere.
        /// 
        /// If the resolver side implements a BAD cache and the name server side receives a query that matches an entry in the resolver side's BAD cache,
        /// the name server side's response depends on the state of the CD bit in the original query.
        /// If the CD bit is set, the name server side should return the data from the BAD cache;
        /// if the CD bit is not set, the name server side must return RCODE 2 (server failure).
        /// 
        /// The intent of the above rule is to provide the raw data to clients that are capable of performing their own signature verification checks
        /// while protecting clients that depend on the resolver side of a security-aware recursive name server to perform such checks.
        /// Several of the possible reasons why signature validation might fail involve conditions
        /// that may not apply equally to the recursive name server and the client that invoked it.
        /// For example, the recursive name server's clock may be set incorrectly, or the client may have knowledge of a relevant island of security
        /// that the recursive name server does not share.
        /// In such cases, "protecting" a client that is capable of performing its own signature validation from ever seeing the "bad" data does not help the client.
        /// </summary>
        public bool IsCheckingDisabled { get; set; }

        /// <summary>
        /// A response of the server that can sign errors or other messages.
        /// </summary>
        public DnsResponseCode ResponseCode { get; set; }

        /// <summary>
        /// The queries resource records.
        /// Typically exactly one query will exist.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<DnsQueryResourceRecord> Queries { get; set; }

        /// <summary>
        /// The answers resource records.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<DnsDataResourceRecord> Answers { get; set; }

        /// <summary>
        /// The authorities resource records.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<DnsDataResourceRecord> Authorities { get; set; }

        /// <summary>
        /// The additionals resource records.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList<DnsDataResourceRecord> Additionals { get; set; }

        /// <summary>
        /// All the resource records by the order the will be written.
        /// </summary>
        public IEnumerable<DnsResourceRecord> ResourceRecords
        {
            get
            {
                return new IEnumerable<DnsResourceRecord>[] {Queries, Answers, Authorities, Additionals}
                    .Where(records => records != null).Aggregate<IEnumerable<DnsResourceRecord>, IEnumerable<DnsResourceRecord>>(
                        null, (current, records) => current == null ? records : current.Concat(records));
            }
        }

        /// <summary>
        /// How to compress the domain names.
        /// </summary>
        public DnsDomainNameCompressionMode DomainNameCompressionMode { get; set; }

        /// <summary>
        /// The number of bytes this layer will take.
        /// </summary>
        public override int Length
        {
            get
            {
                return DnsDatagram.GetLength(ResourceRecords, DomainNameCompressionMode);
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
                              Id, IsResponse, OpCode, IsAuthoritativeAnswer, IsTruncated, IsRecursionDesired, IsRecursionAvailable, FutureUse, IsAuthenticData,
                              IsCheckingDisabled, ResponseCode, Queries, Answers, Authorities, Additionals, DomainNameCompressionMode);
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
                   Id.Equals(other.Id) &&
                   IsQuery.Equals(other.IsQuery) &&
                   OpCode.Equals(other.OpCode) &&
                   IsAuthoritativeAnswer.Equals(other.IsAuthoritativeAnswer) &&
                   IsTruncated.Equals(other.IsTruncated) &&
                   IsRecursionDesired.Equals(other.IsRecursionDesired) &&
                   IsRecursionAvailable.Equals(other.IsRecursionAvailable) &&
                   FutureUse.Equals(other.FutureUse) &&
                   IsAuthenticData.Equals(other.IsAuthenticData) &&
                   IsCheckingDisabled.Equals(other.IsCheckingDisabled) &&
                   ResponseCode.Equals(other.ResponseCode) &&
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