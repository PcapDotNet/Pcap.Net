using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035, 4035.
    /// All communications inside of the domain protocol are carried in a single format called a message. 
    /// The top level format of message is divided into 5 sections (some of which are empty in certain cases) shown below:
    /// <pre>
    /// +-----+----+--------+----+----+----+----+---+----+----+-------+
    /// | bit | 0  | 1-4    | 5  | 6  | 7  | 8  | 9 | 10 | 11 | 12-15 |
    /// +-----+----+--------+----+----+----+----+---+----+----+-------+
    /// | 0   | ID                                                    |
    /// +-----+----+--------+----+----+----+----+---+----+----+-------+
    /// | 16  | QR | Opcode | AA | TC | RD | RA | Z | AD | CD | RCODE |
    /// +-----+----+--------+----+----+----+----+---+----+----+-------+
    /// | 32  | QDCOUNT                                               |
    /// +-----+-------------------------------------------------------+
    /// | 48  | ANCOUNT                                               |
    /// +-----+-------------------------------------------------------+
    /// | 64  | NSCOUNT                                               |
    /// +-----+-------------------------------------------------------+
    /// | 80  | ARCOUNT                                               |
    /// +-----+-------------------------------------------------------+
    /// | 96  | Question - the question for the name server           |
    /// +-----+-------------------------------------------------------+
    /// |     | Answer - RRs answering the question                   |
    /// +-----+-------------------------------------------------------+
    /// |     | Authority - RRs pointing toward an authority          |
    /// +-----+-------------------------------------------------------+
    /// |     | Additional - RRs holding additional information       |
    /// +-----+-------------------------------------------------------+
    /// </pre>
    /// The header section is always present.  
    /// The header includes fields that specify which of the remaining sections are present, 
    /// and also specify whether the message is a query or a response, a standard query or some other opcode, etc.
    /// The names of the sections after the header are derived from their use in standard queries.  
    /// The question section contains fields that describe a question to a name server.  
    /// These fields are a query type (QTYPE), a query class (QCLASS), and a query domain name (QNAME).  
    /// The last three sections have the same format: a possibly empty list of concatenated resource records (RRs).  
    /// The answer section contains RRs that answer the question; the authority section contains RRs that point toward an authoritative name server; 
    /// the additional records section contains RRs which relate to the query, but are not strictly answers for the question.
    /// </summary>
    public sealed class DnsDatagram : Datagram
    {
        private static class Offset
        {
            public const int Id = 0;
            public const int IsResponse = 2;
            public const int OpCode = 2;
            public const int IsAuthoritiveAnswer = 2;
            public const int IsTruncated = 2;
            public const int IsRecusionDesired = 2;
            public const int IsRecusionAvailable = 3;
            public const int FutureUse = 3;
            public const int IsAuthenticData = 3;
            public const int IsCheckingDisabled = 3;
            public const int ResponseCode = 3;
            public const int QueryCount = 4;
            public const int AnswerCount = 6;
            public const int AuthorityCount = 8;
            public const int AdditionalCount = 10;
            public const int Query = 12;
        }

        private static class Mask
        {
            public const byte IsResponse = 0x80;
            public const byte OpCode = 0x78;
            public const byte IsAuthoritiveAnswer = 0x04;
            public const byte IsTruncated = 0x02;
            public const byte IsRecusionDesired = 0x01;
            public const byte IsRecursionAvailable = 0x80;
            public const byte FutureUse = 0x40;
            public const byte IsAuthenticData = 0x20;
            public const byte IsCheckingDisabled = 0x10;
            public const ushort ResponseCode = 0x000F;
        }

        private static class Shift
        {
            public const int OpCode = 3;
        }

        /// <summary>
        /// The number of bytes the DNS header takes.
        /// </summary>
        public const int HeaderLength = 12;

        /// <summary>
        /// A 16 bit identifier assigned by the program that generates any kind of query.  
        /// This identifier is copied the corresponding reply and can be used by the requester to match up replies to outstanding queries.
        /// </summary>
        public ushort Id
        {
            get { return ReadUShort(Offset.Id, Endianity.Big); }
        }

        /// <summary>
        /// A one bit field that specifies whether this message is a query (0), or a response (1).
        /// </summary>
        public bool IsResponse
        {
            get { return ReadBool(Offset.IsResponse, Mask.IsResponse); }
        }

        /// <summary>
        /// Specifies whether this message is a query or a response.
        /// </summary>
        public bool IsQuery
        {
            get { return !IsResponse; }
        }

        /// <summary>
        /// Specifies kind of query in this message.  
        /// This value is set by the originator of a query and copied into the response.
        /// </summary>
        public DnsOpCode OpCode
        {
            get { return (DnsOpCode)((this[Offset.OpCode] & Mask.OpCode) >> Shift.OpCode); }
        }

        /// <summary>
        /// This bit is valid in responses, and specifies that the responding name server is an authority for the domain name in question section.
        /// Note that the contents of the answer section may have multiple owner names because of aliases.  
        /// The AA bit corresponds to the name which matches the query name, or the first owner name in the answer section.
        /// </summary>
        public bool IsAuthoritativeAnswer
        {
            get { return ReadBool(Offset.IsAuthoritiveAnswer, Mask.IsAuthoritiveAnswer); }
        }

        /// <summary>
        /// Specifies that this message was truncated due to length greater than that permitted on the transmission channel.
        /// </summary>
        public bool IsTruncated
        {
            get { return ReadBool(Offset.IsTruncated, Mask.IsTruncated); }
        }

        /// <summary>
        /// This bit may be set in a query and is copied into the response.  
        /// If RD is set, it directs the name server to pursue the query recursively. 
        /// Recursive query support is optional.
        /// </summary>
        public bool IsRecursionDesired
        {
            get { return ReadBool(Offset.IsRecusionDesired, Mask.IsRecusionDesired); }
        }

        /// <summary>
        /// This be is set or cleared in a response, and denotes whether recursive query support is available in the name server.
        /// </summary>
        public bool IsRecursionAvailable
        {
            get { return ReadBool(Offset.IsRecusionAvailable, Mask.IsRecursionAvailable); }
        }

        /// <summary>
        /// Reserved for future use.  
        /// Must be false in all queries and responses.
        /// </summary>
        public bool FutureUse
        {
            get { return ReadBool(Offset.FutureUse, Mask.FutureUse); }
        }

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
        public bool IsAuthenticData
        {
            get { return ReadBool(Offset.IsAuthenticData, Mask.IsAuthenticData); }
        }

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
        public bool IsCheckingDisabled
        {
            get { return ReadBool(Offset.IsCheckingDisabled, Mask.IsCheckingDisabled); }
        }

        public DnsResponseCode ResponseCode
        {
            get { return (DnsResponseCode)(this[Offset.ResponseCode] & Mask.ResponseCode); }
        }

        /// <summary>
        /// An unsigned 16 bit integer specifying the number of entries in the question section.
        /// </summary>
        public ushort QueryCount
        {
            get { return ReadUShort(Offset.QueryCount, Endianity.Big); }
        }

        /// <summary>
        /// An unsigned 16 bit integer specifying the number of resource records in the answer section.
        /// </summary>
        public ushort AnswerCount
        {
            get { return ReadUShort(Offset.AnswerCount, Endianity.Big); }
        }

        /// <summary>
        /// An unsigned 16 bit integer specifying the number of name server resource records in the authority records section.
        /// </summary>
        public ushort AuthorityCount
        {
            get { return ReadUShort(Offset.AuthorityCount, Endianity.Big); }
        }

        /// <summary>
        /// An unsigned 16 bit integer specifying the number of resource records in the additional records section.
        /// </summary>
        public ushort AdditionalCount
        {
            get { return ReadUShort(Offset.AdditionalCount, Endianity.Big); }
        }

        public ReadOnlyCollection<DnsQueryResourceRecord> Queries
        {
            get
            {
                ParseQueries();
                return _queries;
            }
        }

        public ReadOnlyCollection<DnsDataResourceRecord> Answers
        {
            get
            {
                ParseAnswers();
                return _answers;
            }
        }

        public ReadOnlyCollection<DnsDataResourceRecord> Authorities
        {
            get
            {
                ParseAuthorities();
                return _authorities;
            }
        }

        public ReadOnlyCollection<DnsDataResourceRecord> Additionals
        {
            get
            {
                ParseAdditionals();
                return _additionals;
            }
        }

        public IEnumerable<DnsResourceRecord> ResourceRecords
        {
            get { return Queries.Cast<DnsResourceRecord>().Concat(DataResourceRecords); }
        }

        public IEnumerable<DnsDataResourceRecord> DataResourceRecords
        {
            get { return Answers.Concat(Authorities).Concat(Additionals); }
        }

        public DnsOptResourceRecord OptionsRecord
        {
            get
            {
                ParseAdditionals();
                return _options;
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new DnsLayer
                   {
                       Id = Id,
                       IsQuery = IsQuery,
                       OpCode = OpCode,
                       IsAuthoritativeAnswer = IsAuthoritativeAnswer,
                       IsTruncated = IsTruncated,
                       IsRecursionDesired = IsRecursionDesired,
                       IsRecursionAvailable = IsRecursionAvailable,
                       FutureUse = FutureUse,
                       IsAuthenticData = IsAuthenticData,
                       IsCheckingDisabled = IsCheckingDisabled,
                       ResponseCode = ResponseCode,
                       Queries = Queries.ToList(),
                       Answers = Answers.ToList(),
                       Authorities = Authorities.ToList(),
                       Additionals = Additionals.ToList(),
                   };
        }

        protected override bool CalculateIsValid()
        {
            if (_isValid == null)
            {
                _isValid = Length >= HeaderLength &&
                           QueryCount == Queries.Count &&
                           AnswerCount == Answers.Count &&
                           AuthorityCount == Authorities.Count &&
                           AdditionalCount == Additionals.Count;
            }
            return _isValid.Value;
        }

        internal DnsDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static int GetLength(IEnumerable<DnsResourceRecord> resourceRecords, DnsDomainNameCompressionMode domainNameCompressionMode)
        {
            int length = HeaderLength;
            DnsDomainNameCompressionData compressionData = new DnsDomainNameCompressionData(domainNameCompressionMode);
            if (resourceRecords != null)
            {
                foreach (DnsResourceRecord record in resourceRecords)
                    length += record.GetLength(compressionData, length);
            }

            return length;
        }

        internal static void Write(byte[] buffer, int offset,
                                   ushort id, bool isResponse, DnsOpCode opCode, bool isAuthoritiveAnswer, bool isTruncated,
                                   bool isRecursionDesired, bool isRecursionAvailable, bool futureUse, bool isAuthenticData, bool isCheckingDisabled,
                                   DnsResponseCode responseCode, IList<DnsQueryResourceRecord> queries, IList<DnsDataResourceRecord> answers,
                                   IList<DnsDataResourceRecord> authorities, IList<DnsDataResourceRecord> additionals,
                                   DnsDomainNameCompressionMode domainNameCompressionMode)
        {
            buffer.Write(offset + Offset.Id, id, Endianity.Big);
            byte flags0 = 0;
            if (isResponse)
                flags0 |= Mask.IsResponse;
            flags0 |= (byte)((((byte)opCode) << Shift.OpCode) & Mask.OpCode);
            if (isAuthoritiveAnswer)
                flags0 |= Mask.IsAuthoritiveAnswer;
            if (isTruncated)
                flags0 |= Mask.IsTruncated;
            if (isRecursionDesired)
                flags0 |= Mask.IsRecusionDesired;
            buffer.Write(offset + Offset.IsResponse, flags0);
            byte flags1 = 0;
            if (isRecursionAvailable)
                flags1 |= Mask.IsRecursionAvailable;
            if (futureUse)
                flags1 |= Mask.FutureUse;
            if (isAuthenticData)
                flags1 |= Mask.IsAuthenticData;
            if (isCheckingDisabled)
                flags1 |= Mask.IsCheckingDisabled;
            flags1 |= (byte)((ushort)responseCode & Mask.ResponseCode);
            buffer.Write(offset + Offset.IsRecusionAvailable, flags1);
            DnsDomainNameCompressionData compressionData = new DnsDomainNameCompressionData(domainNameCompressionMode);
            int recordOffset = HeaderLength;
            if (queries != null)
            {
                buffer.Write(offset + Offset.QueryCount, (ushort)queries.Count, Endianity.Big);
                foreach (DnsQueryResourceRecord record in queries)
                    recordOffset += record.Write(buffer, offset, compressionData, recordOffset);
            }
            if (answers != null)
            {
                buffer.Write(offset + Offset.AnswerCount, (ushort)answers.Count, Endianity.Big);
                foreach (DnsDataResourceRecord record in answers)
                    recordOffset += record.Write(buffer, offset, compressionData, recordOffset);
            }
            if (authorities != null)
            {
                buffer.Write(offset + Offset.AuthorityCount, (ushort)authorities.Count, Endianity.Big);
                foreach (DnsDataResourceRecord record in authorities)
                    recordOffset += record.Write(buffer, offset, compressionData, recordOffset);
            }
            if (additionals != null)
            {
                buffer.Write(offset + Offset.AdditionalCount, (ushort)additionals.Count, Endianity.Big);
                foreach (DnsDataResourceRecord record in additionals)
                    recordOffset += record.Write(buffer, offset, compressionData, recordOffset);
            }
        }

        private int QueriesOffset
        {
            get { return HeaderLength; }
        }

        private int AnswersOffset
        {
            get
            {
                ParseQueries();
                return _answersOffset;
            }
        }

        private int AuthoritiesOffset
        {
            get
            {
                ParseAnswers();
                return _authoritiesOffset;
            }
        }

        private int AdditionalsOffset
        {
            get
            {
                ParseAuthorities();
                return _additionalsOffset;
            }
        }

        private void ParseQueries()
        {
            ParseRecords(QueriesOffset, () => QueryCount, DnsQueryResourceRecord.Parse, ref _queries, ref _answersOffset);
        }

        private void ParseAnswers()
        {
            ParseRecords(AnswersOffset, () => AnswerCount, DnsDataResourceRecord.Parse, ref _answers, ref _authoritiesOffset);
        }

        private void ParseAuthorities()
        {
            ParseRecords(AuthoritiesOffset, () => AuthorityCount, DnsDataResourceRecord.Parse, ref _authorities, ref _additionalsOffset);
        }

        private void ParseAdditionals()
        {
            int nextOffset = 0;
            if (ParseRecords(AdditionalsOffset, () => AdditionalCount, DnsDataResourceRecord.Parse, ref _additionals, ref nextOffset) &&
                _additionals != null)
            {
                _options = (DnsOptResourceRecord)_additionals.FirstOrDefault(additional => additional.Type == DnsType.Opt);
            }
        }

        private delegate TRecord ParseRecord<out TRecord>(DnsDatagram dns, int offset, out int numBytesRead);

        private bool ParseRecords<TRecord>(int offset, Func<ushort> countDelegate, ParseRecord<TRecord> parseRecord,
                                           ref ReadOnlyCollection<TRecord> parsedRecords, ref int nextOffset) where TRecord : DnsResourceRecord
        {
            if (parsedRecords == null && Length >= offset)
            {
                ushort count = countDelegate();
                List<TRecord> records = new List<TRecord>(count);
                for (int i = 0; i != count; ++i)
                {
                    int numBytesRead;
                    TRecord record = parseRecord(this, offset, out numBytesRead);
                    if (record == null)
                    {
                        offset = 0;
                        break;
                    }
                    records.Add(record);
                    offset += numBytesRead;
                }
                parsedRecords = new ReadOnlyCollection<TRecord>(records.ToArray());
                nextOffset = offset;

                return true;
            }
            return false;
        }

        private ReadOnlyCollection<DnsQueryResourceRecord> _queries;
        private ReadOnlyCollection<DnsDataResourceRecord> _answers;
        private ReadOnlyCollection<DnsDataResourceRecord> _authorities;
        private ReadOnlyCollection<DnsDataResourceRecord> _additionals;
        private DnsOptResourceRecord _options;

        private int _answersOffset;
        private int _authoritiesOffset;
        private int _additionalsOffset;

        private bool? _isValid;
    }
}
