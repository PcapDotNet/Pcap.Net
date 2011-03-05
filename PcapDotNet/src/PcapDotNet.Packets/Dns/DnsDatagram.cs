using System.Collections.ObjectModel;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// All communications inside of the domain protocol are carried in a single format called a message. 
    /// The top level format of message is divided into 5 sections (some of which are empty in certain cases) shown below:
    /// <pre>
    /// +-----+----+--------+----+----+----+----+------+--------+
    /// | bit | 0  | 1-4    | 5  | 6  | 7  | 8  | 9-11 | 12-15  |
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
    public class DnsDatagram : Datagram
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
            public const byte FutureUse = 0x70;
            public const byte ResponseCode = 0x0F;
        }

        private static class Shift
        {
            public const int Opcode = 3;
            public const int FutureUse = 4;
        }

        /// <summary>
        /// The number of bytes the DNS header takes.
        /// </summary>
        public const int HeaderLength = 10;

        public ushort Id
        {
            get { return ReadUShort(Offset.Id, Endianity.Big); }
        }

        public bool IsResponse
        {
            get { return ReadBool(Offset.IsResponse, Mask.IsResponse); }
        }

        public bool IsQuery
        {
            get { return !IsResponse; }
        }

        public byte Opcode
        {
            get { return (byte)((this[Offset.Opcode] & Mask.Opcode) >> Shift.Opcode); }
        }

        public bool IsAuthoritiveAnswer
        {
            get { return ReadBool(Offset.IsAuthoritiveAnswer, Mask.IsAuthoritiveAnswer); }
        }

        public bool IsTruncated
        {
            get { return ReadBool(Offset.IsTruncated, Mask.IsTruncated); }
        }

        public bool IsRecusionDesired
        {
            get { return ReadBool(Offset.IsRecusionDesired, Mask.IsRecusionDesired); }
        }

        public bool IsRecusionAvailable
        {
            get { return ReadBool(Offset.IsRecusionAvailable, Mask.IsRecusionAvailable); }
        }
        
        public byte FutureUse
        {
            get { return (byte)((this[Offset.FutureUse] & Mask.FutureUse) >> Shift.FutureUse); }
        }

        public byte ResponseCode
        {
            get { return (byte)(this[Offset.ResponseCode] & Mask.ResponseCode); }
        }

        public ushort QueryCount
        {
            get { return ReadUShort(Offset.QueryCount, Endianity.Big); }
        }

        public ushort AnswerCount
        {
            get { return ReadUShort(Offset.AnswerCount, Endianity.Big); }
        }

        public ushort AuthorityCount
        {
            get { return ReadUShort(Offset.AuthorityCount, Endianity.Big); }
        }

        public ushort AdditionalCount
        {
            get { return ReadUShort(Offset.AdditionalCount, Endianity.Big); }
        }
        /*
        public ReadOnlyCollection<DnsResourceRecord> Query
        {
            
        }
         */

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        /*
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
        */
        /*
        protected override bool CalculateIsValid()
        {
            return Length >= HeaderBaseLength && Length == HeaderLength;
        }
        */
        internal DnsDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}