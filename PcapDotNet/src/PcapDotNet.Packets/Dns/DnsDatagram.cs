using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// All communications inside of the domain protocol are carried in a single format called a message. 
    /// The top level format of message is divided into 5 sections (some of which are empty in certain cases) shown below:
    /// <pre>
    /// +-----+----+--------+----+----+----+----+------+--------+
    /// | bit | 0  | 1-4    | 5  | 6  | 7  | 8  | 9-10 | 11-15  |
    /// +-----+----+--------+----+----+----+----+------+--------+
    /// | 0   | ID                                              |
    /// +-----+----+--------+----+----+----+----+------+--------+
    /// | 16  | QR | Opcode | AA | TC | RD | RA | Z    | RCODE  |
    /// +-----+----+--------+----+----+----+----+------+--------+
    /// | 32  | QDCOUNT                                         |
    /// +-----+-------------------------------------------------+
    /// | 48  | ANCOUNT                                         |
    /// +-----+-------------------------------------------------+
    /// | 64  | NSCOUNT                                         |
    /// +-----+-------------------------------------------------+
    /// | 80  | ARCOUNT                                         |
    /// +-----+-------------------------------------------------+
    /// | 96  | Question - the question for the name server     |
    /// +-----+-------------------------------------------------+
    /// |     | Answer - RRs answering the question             |
    /// +-----+-------------------------------------------------+
    /// |     | Authority - RRs pointing toward an authority    |
    /// +-----+-------------------------------------------------+
    /// |     | Additional - RRs holding additional information |
    /// +-----+-------------------------------------------------+
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
            public const int Opcode = 2;
            public const int IsAuthoritiveAnswer = 2;
            public const int IsTruncated = 2;
            public const int IsRecusionDesired = 2;
            public const int IsRecusionAvailable = 3;
            public const int FutureUse = 3;
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
            public const byte Opcode = 0x78;
            public const byte IsAuthoritiveAnswer = 0x04;
            public const byte IsTruncated = 0x02;
            public const byte IsRecusionDesired = 0x01;
            public const byte IsRecusionAvailable = 0x80;
            public const byte FutureUse = 0x60;
            public const byte ResponseCode = 0x1F;
        }

        private static class Shift
        {
            public const int Opcode = 3;
            public const int FutureUse = 5;
        }

        /// <summary>
        /// The number of bytes the DNS header takes.
        /// </summary>
        public const int HeaderLength = 12;

        public const byte MaxFutureUse = 3;

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
        public DnsOpcode Opcode
        {
            get { return (DnsOpcode)((this[Offset.Opcode] & Mask.Opcode) >> Shift.Opcode); }
        }

        /// <summary>
        /// This bit is valid in responses, and specifies that the responding name server is an authority for the domain name in question section.
        /// Note that the contents of the answer section may have multiple owner names because of aliases.  
        /// The AA bit corresponds to the name which matches the query name, or the first owner name in the answer section.
        /// </summary>
        public bool IsAuthoritiveAnswer
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
        public bool IsRecusionDesired
        {
            get { return ReadBool(Offset.IsRecusionDesired, Mask.IsRecusionDesired); }
        }

        /// <summary>
        /// This be is set or cleared in a response, and denotes whether recursive query support is available in the name server.
        /// </summary>
        public bool IsRecusionAvailable
        {
            get { return ReadBool(Offset.IsRecusionAvailable, Mask.IsRecusionAvailable); }
        }

        /// <summary>
        /// Reserved for future use.  
        /// Must be zero in all queries and responses.
        /// </summary>
        public byte FutureUse
        {
            get { return (byte)((this[Offset.FutureUse] & Mask.FutureUse) >> Shift.FutureUse); }
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

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new DnsLayer
            {
                Id = Id,
                IsQuery = IsQuery,
                Opcode = Opcode,
                IsAuthoritiveAnswer = IsAuthoritiveAnswer,
                IsTruncated = IsTruncated,
                IsRecusionDesired = IsRecusionDesired,
                IsRecusionAvailable = IsRecusionAvailable,
                FutureUse = FutureUse,
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
                                   ushort id, bool isResponse, DnsOpcode opcode, bool isAuthoritiveAnswer, bool isTruncated,
                                   bool isRecursionDesired, bool isRecursionAvailable, byte futureUse, DnsResponseCode responseCode,
                                   List<DnsQueryResourceRecord> queries, List<DnsDataResourceRecord> answers, 
                                   List<DnsDataResourceRecord> authorities, List<DnsDataResourceRecord> additionals,
                                   DnsDomainNameCompressionMode domainNameCompressionMode)
        {
            buffer.Write(offset + Offset.Id, id, Endianity.Big);
            byte flags0 = 0;
            if (isResponse)
                flags0 |= Mask.IsResponse;
            flags0 |= (byte)((((byte)opcode) << Shift.Opcode) & Mask.Opcode);
            if (isAuthoritiveAnswer)
                flags0 |= Mask.IsAuthoritiveAnswer;
            if (isTruncated)
                flags0 |= Mask.IsTruncated;
            if (isRecursionDesired)
                flags0 |= Mask.IsRecusionDesired;
            buffer.Write(offset + Offset.IsResponse, flags0);
            byte flags1 = 0;
            if (isRecursionAvailable)
                flags1 |= Mask.IsRecusionAvailable;
            flags1 |= (byte)((futureUse << Shift.FutureUse) & Mask.FutureUse);
            flags1 |= (byte)((byte)responseCode & Mask.ResponseCode);
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
            ParseRecords(AdditionalsOffset, () => AdditionalCount, DnsDataResourceRecord.Parse, ref _additionals, ref nextOffset);
        }

        private delegate TRecord ParseRecord<out TRecord>(DnsDatagram dns, int offset, out int numBytesRead);

        private void ParseRecords<TRecord>(int offset, Func<ushort> countDelegate, ParseRecord<TRecord> parseRecord,
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
            }
        }

        private ReadOnlyCollection<DnsQueryResourceRecord> _queries;
        private ReadOnlyCollection<DnsDataResourceRecord> _answers;
        private ReadOnlyCollection<DnsDataResourceRecord> _authorities;
        private ReadOnlyCollection<DnsDataResourceRecord> _additionals;

        private int _answersOffset;
        private int _authoritiesOffset;
        private int _additionalsOffset;

        private bool? _isValid;
    }
}
