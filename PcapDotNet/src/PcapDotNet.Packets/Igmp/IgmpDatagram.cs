using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// RFC 1112.
    /// Version 1 (query or report):
    /// <pre>
    /// +-----+---------+------+--------+----------+
    /// | Bit | 0-3     | 4-7  | 8-15   | 16-31    |
    /// +-----+---------+------+--------+----------+
    /// | 0   | Version | Type | Unused | Checksum |
    /// +-----+---------+------+--------+----------+
    /// | 32  | Group Address                      |
    /// +-----+------------------------------------+
    /// </pre>
    /// 
    /// RFC 2236.
    /// Version 2 (query, report or leave group):
    /// <pre>
    /// +-----+------+---------------+----------+
    /// | Bit | 0-7  | 8-15          | 16-31    |
    /// +-----+------+---------------+----------+
    /// | 0   | Type | Max Resp Time | Checksum |
    /// +-----+------+---------------+----------+
    /// | 32  | Group Address                   |
    /// +-----+---------------------------------+
    /// </pre>
    /// 
    /// RFC 3376.
    /// Version 3 query:
    /// <pre>
    /// +-----+------+---+-----+---------------+-----------------------+
    /// | Bit | 0-3  | 4 | 5-7 | 8-15          | 16-31                 |
    /// +-----+------+---+-----+---------------+-----------------------+
    /// | 0   | Type = 0x11    | Max Resp Code | Checksum              |
    /// +-----+----------------+---------------+-----------------------+
    /// | 32  | Group Address                                          |
    /// +-----+------+---+-----+---------------+-----------------------+
    /// | 64  | Resv | S | QRV | QQIC          | Number of Sources (N) |
    /// +-----+------+---+-----+---------------+-----------------------+
    /// | 96  | Source Address [1]                                     |
    /// +-----+--------------------------------------------------------+
    /// | 128 | Source Address [2]                                     |
    /// +-----+--------------------------------------------------------+
    /// .     .                         .                              .
    /// .     .                         .                              .
    /// +-----+--------------------------------------------------------+
    /// | 64  | Source Address [N]                                     |
    /// | +   |                                                        |
    /// | 32N |                                                        |
    /// +-----+--------------------------------------------------------+
    /// </pre>
    /// 
    /// RFC 3376.
    /// Version 3 report:
    /// <pre>
    /// +-----+-------------+----------+-----------------------------+
    /// | Bit | 0-7         | 8-15     | 16-31                       |
    /// +-----+-------------+----------+-----------------------------+
    /// | 0   | Type = 0x22 | Reserved | Checksum                    |
    /// +-----+-------------+----------+-----------------------------+
    /// | 32  | Reserved               | Number of Group Records (M) |
    /// +-----+------------------------+-----------------------------+
    /// | 64  | Group Record [1]                                     |
    /// .     .                                                      .
    /// .     .                                                      .
    /// .     .                                                      .
    /// |     |                                                      |
    /// +-----+------------------------------------------------------+
    /// |     | Group Record [2]                                     |
    /// .     .                                                      .
    /// .     .                                                      .
    /// .     .                                                      .
    /// |     |                                                      |
    /// +-----+------------------------------------------------------+
    /// |     |                         .                            |
    /// .     .                         .                            .
    /// |     |                         .                            |
    /// +-----+------------------------------------------------------+
    /// |     | Group Record [M]                                     |
    /// .     .                                                      .
    /// .     .                                                      .
    /// .     .                                                      .
    /// |     |                                                      |
    /// +-----+------------------------------------------------------+
    /// </pre>
    /// </summary>
    public class IgmpDatagram : Datagram
    {
        /// <summary>
        /// The number of bytes the IGMP header takes for all messages but query version 3.
        /// All the bytes but the records of the report version 3.
        /// </summary>
        public const int HeaderLength = 8;

        /// <summary>
        /// The number of bytes the query version 3 IGMP message header takes.
        /// All the bytes but the source addresses.
        /// </summary>
        public const int QueryVersion3HeaderLength = 12;

        private static class Offset
        {
            public const int MessageType = 0;
            public const int MaxResponseCode = 1;
            public const int Checksum = 2;
            public const int GroupAddress = 4;

            // Version 3 query
            public const int IsSuppressRouterSideProcessing = 8;
            public const int QueryRobustnessVariable = 8;
            public const int QueryIntervalCode = 9;
            public const int NumberOfSources = 10;
            public const int SourceAddresses = 12;

            // Version 3 report
            public const int NumberOfGroupRecords = 6;
            public const int GroupRecords = 8;
        }

        /// <summary>
        /// The maximum value for the query robustness varialbe.
        /// </summary>
        public const byte MaxQueryRobustnessVariable = 0x07;

        /// <summary>
        /// The Max Resp Code field specifies the maximum time allowed before sending a responding report.  
        /// </summary>
        public static TimeSpan MaxMaxResponseTime
        {
            get { return _maxMaxResponseTime; }
        }

        /// <summary>
        /// The maximum value for the max response time in version 3 messages.
        /// </summary>
        public static TimeSpan MaxVersion3MaxResponseTime
        {
            get { return _maxVersion3MaxResponseTime; }
        }

        /// <summary>
        /// The maximum value for the query interval.
        /// </summary>
        public static TimeSpan MaxQueryInterval
        {
            get { return _maxQueryInterval; }
        }

        /// <summary>
        /// The type of the IGMP message of concern to the host-router interaction.
        /// </summary>
        public IgmpMessageType MessageType
        {
            get { return (IgmpMessageType)this[Offset.MessageType]; }
        }

        /// <summary>
        /// The version of the IGMP protocol for this datagram.
        /// </summary>
        public int Version
        {
            get
            {
                switch (MessageType)
                {
                    case IgmpMessageType.MembershipQuery:
                        switch (QueryVersion)
                        {
                            case IgmpQueryVersion.Version1:
                                return 1;

                            case IgmpQueryVersion.Version2:
                                return 2;

                            case IgmpQueryVersion.Version3:
                                return 3;

                            default:
                                throw new InvalidOperationException("Invalid QueryVersion " + QueryVersion);
                        }

                    case IgmpMessageType.MembershipReportVersion1:
                        return 1;

                    case IgmpMessageType.MembershipReportVersion2:
                        return 2;

                    case IgmpMessageType.MembershipReportVersion3:
                        return 3; 

                    case IgmpMessageType.LeaveGroupVersion2:
                        return 2;

                    default:
                        throw new InvalidOperationException("Invalid MessageType " + MessageType);
                }
            }
        }

        /// <summary>
        /// The IGMP version of a Membership Query message is determined as follows:
        /// <list type="bullet">
        ///   <item>IGMPv1 Query: length = 8 octets AND Max Resp Code field is zero.</item>
        ///   <item>IGMPv2 Query: length = 8 octets AND Max Resp Code field is non-zero.</item>
        ///   <item>IGMPv3 Query: length >= 12 octets.</item>
        /// </list>
        /// If the type is not a query, None will be returned.
        /// If the query message do not match any of the above conditions (e.g., a Query of length 10 octets) Unknown will be returned.
        /// </summary>
        public IgmpQueryVersion QueryVersion
        {
            get
            {
                if (MessageType != IgmpMessageType.MembershipQuery)
                    return IgmpQueryVersion.None;

                if (Length >= QueryVersion3HeaderLength)
                    return IgmpQueryVersion.Version3;

                if (Length != HeaderLength)
                    return IgmpQueryVersion.Unknown;

                if (MaxResponseCode == 0)
                    return IgmpQueryVersion.Version1;

                return IgmpQueryVersion.Version2;
            }
        }

        /// <summary>
        /// The Max Resp Code field specifies the maximum time allowed before sending a responding report.  
        /// The actual time allowed, called the Max Resp Time, is represented in units of 1/10 second and is derived from the Max Resp Code as follows:
        /// <list type="bullet">
        ///   <item>If Max Resp Code &lt; 128, Max Resp Time = Max Resp Code.</item>
        ///   <item>
        ///     If Max Resp Code >= 128, Max Resp Code represents a floating-point value as follows:
        ///     <pre>
        ///     0 1 2 3 4 5 6 7
        ///     +-+-+-+-+-+-+-+-+
        ///     |1| exp | mant  |
        ///     +-+-+-+-+-+-+-+-+
        ///     </pre>
        ///     Max Resp Time = (mant | 0x10) &lt;&lt; (exp + 3).
        ///   </item>
        /// </list>
        /// 
        /// <para>
        /// Small values of Max Resp Time allow IGMPv3 routers to tune the "leave latency" 
        /// (the time between the moment the last host leaves a group and the moment the routing protocol is notified that there are no more members).
        /// Larger values, especially in the exponential range, allow tuning of the burstiness of IGMP traffic on a network.
        /// </para>
        /// </summary>
        public byte MaxResponseCode
        {
            get { return this[Offset.MaxResponseCode]; }
        }

        /// <summary>
        /// The actual time allowed, called the Max Resp Time, is represented in units of 1/10 second and is derived from the Max Resp Code as follows:
        /// <list type="bullet">
        ///   <item>If the query version is 1 or 2 or if Max Resp Code &lt; 128, Max Resp Time = Max Resp Code.</item>
        ///   <item>
        ///     If Max Resp Code >= 128, Max Resp Code represents a floating-point value as follows:
        ///     <pre>
        ///      0 1 2 3 4 5 6 7
        ///     +-+-+-+-+-+-+-+-+
        ///     |1| exp | mant  |
        ///     +-+-+-+-+-+-+-+-+
        ///     </pre>
        ///     Max Resp Time = (mant | 0x10) &lt;&lt; (exp + 3).
        ///   </item>
        /// </list>
        /// </summary>
        public TimeSpan MaxResponseTime
        {
            get
            {
                byte maxResponseCode = MaxResponseCode;
                int numTenthOfASecond =
                    ((maxResponseCode < 128 || MessageType != IgmpMessageType.MembershipQuery || QueryVersion != IgmpQueryVersion.Version3)
                         ? maxResponseCode
                         : CodeToValue(maxResponseCode));

                return TimeSpan.FromMilliseconds(100 * numTenthOfASecond);
            }
        }

        /// <summary>
        /// The Checksum is the 16-bit one's complement of the one's complement sum of the whole IGMP message (the entire IP payload).  
        /// For computing the checksum, the Checksum field is set to zero.  
        /// When receiving packets, the checksum MUST be verified before processing a packet.
        /// </summary>
        public ushort Checksum
        {
            get { return ReadUShort(Offset.Checksum, Endianity.Big); }
        }

        /// <summary>
        /// True iff the checksum value is correct according to the datagram data.
        /// </summary>
        public bool IsChecksumCorrect
        {
            get
            {
                if (_isChecksumCorrect == null)
                    _isChecksumCorrect = (CalculateChecksum() == Checksum);
                return _isChecksumCorrect.Value;
            }
        }

        /// <summary>
        /// The Group Address field is set to zero when sending a General Query, 
        /// and set to the IP multicast address being queried when sending a Group-Specific Query or Group-and-Source-Specific Query.
        /// In a Membership Report of version 1 or 2 or Leave Group message, the group address field holds the IP multicast group address of the group being reported or left.
        /// In a Membership Report of version 3 this field is meaningless.
        /// </summary>
        public IpV4Address GroupAddress
        {
            get { return ReadIpV4Address(Offset.GroupAddress, Endianity.Big); }
        }

        /// <summary>
        /// When set to one, the S Flag indicates to any receiving multicast routers that they are to suppress the normal timer updates they perform upon hearing a Query.  
        /// It does not, however, suppress the querier election or the normal "host-side" processing of a Query 
        /// that a router may be required to perform as a consequence of itself being a group member.
        /// </summary>
        /// <remarks>
        /// Valid only on query of version 3.
        /// </remarks>
        public bool IsSuppressRouterSideProcessing
        {
            get { return ((this[Offset.IsSuppressRouterSideProcessing] >> 3) & 0x01) == 0x01; }
        }

        /// <summary>
        /// If non-zero, the QRV field contains the [Robustness Variable] value used by the querier, i.e., the sender of the Query.  
        /// If the querier's [Robustness Variable] exceeds 7, the maximum value of the QRV field, the QRV is set to zero.  
        /// Routers adopt the QRV value from the most recently received Query as their own [Robustness Variable] value, 
        /// unless that most recently received QRV was zero, in which case the receivers use the default [Robustness Variable] value or a statically configured value.
        /// </summary>
        /// <remarks>
        /// Valid only on query of version 3.
        /// </remarks>
        public byte QueryRobustnessVariable
        {
            get { return (byte)(this[Offset.QueryRobustnessVariable] & 0x07); }
        }

        /// <summary>
        /// The Querier's Query Interval Code field specifies the [Query Interval] used by the querier.  
        /// The actual interval, called the Querier's Query Interval (QQI), is represented in units of seconds and is derived from the Querier's Query Interval Code as follows:
        /// <list type="bullet">
        ///   <item>If QQIC &lt; 128, QQI = QQIC</item>
        ///   <item>
        ///     If QQIC >= 128, QQIC represents a floating-point value as follows:
        ///     <pre>
        ///      0 1 2 3 4 5 6 7
        ///     +-+-+-+-+-+-+-+-+
        ///     |1| exp | mant  |
        ///     +-+-+-+-+-+-+-+-+
        ///     </pre>
        ///     QQI = (mant | 0x10) &lt;&lt; (exp + 3)
        ///   </item>
        /// </list>
        /// Multicast routers that are not the current querier adopt the QQI value from the most recently received Query as their own [Query Interval] value, 
        /// unless that most recently received QQI was zero, in which case the receiving routers use the default [Query Interval] value.
        /// </summary>
        /// <remarks>
        /// Valid only on query of version 3.
        /// </remarks>
        public byte QueryIntervalCode
        {
            get { return this[Offset.QueryIntervalCode]; }
        }

        /// <summary>
        /// The actual interval, called the Querier's Query Interval (QQI), is represented in units of seconds and is derived from the Querier's Query Interval Code as follows:
        /// <list type="bullet">
        ///   <item>If QQIC &lt; 128, QQI = QQIC</item>
        ///   <item>
        ///     If QQIC >= 128, QQIC represents a floating-point value as follows:
        ///     <pre>
        ///      0 1 2 3 4 5 6 7
        ///     +-+-+-+-+-+-+-+-+
        ///     |1| exp | mant  |
        ///     +-+-+-+-+-+-+-+-+
        ///     </pre>
        ///     QQI = (mant | 0x10) &lt;&lt; (exp + 3)
        ///   </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Valid only on query of version 3.
        /// </remarks>
        public TimeSpan QueryInterval
        {
            get 
            {
                int numSeconds = QueryIntervalCode < 128
                                     ? QueryIntervalCode
                                     : CodeToValue(QueryIntervalCode);

                return TimeSpan.FromSeconds(numSeconds);
            }
        }

        /// <summary>
        /// The Number of Sources (N) field specifies how many source addresses are present in the Query.  
        /// This number is zero in a General Query or a Group-Specific Query, and non-zero in a Group-and-Source-Specific Query.  
        /// This number is limited by the MTU of the network over which the Query is transmitted.  
        /// For example, on an Ethernet with an MTU of 1500 octets, the IP header including the Router Alert option consumes 24 octets, 
        /// and the IGMP fields up to including the Number of Sources (N) field consume 12 octets, leaving 1464 octets for source addresses, 
        /// which limits the number of source addresses to 366 (1464/4).
        /// </summary>
        /// <remarks>
        /// Valid only on query of version 3.
        /// </remarks>
        public ushort NumberOfSources
        {
            get { return ReadUShort(Offset.NumberOfSources, Endianity.Big); }
        }

        /// <summary>
        /// The Source Address [i] fields are a vector of n IP unicast addresses,
        /// where n is the value in the Number of Sources (N) field.
        /// </summary>
        /// <remarks>
        /// Valid only on query of version 3.
        /// </remarks>
        public ReadOnlyCollection<IpV4Address> SourceAddresses
        {
            get
            {
                if (_sourceAddresses == null)
                {
                    IpV4Address[] sourceAddresses = new IpV4Address[NumberOfSources];
                    for (int i = 0; i != sourceAddresses.Length; ++i)
                        sourceAddresses[i] = ReadIpV4Address(Offset.SourceAddresses + IpV4Address.SizeOf * i, Endianity.Big);
                    _sourceAddresses = new ReadOnlyCollection<IpV4Address>(sourceAddresses);
                }

                return _sourceAddresses;
            }
        }

        /// <summary>
        /// The Number of Group Records (M) field specifies how many Group Records are present in this Report.
        /// </summary>
        /// <remarks>
        /// Valid only on report of version 3.
        /// </remarks>
        public ushort NumberOfGroupRecords
        {
            get { return ReadUShort(Offset.NumberOfGroupRecords, Endianity.Big); }
        }

        /// <summary>
        /// Each Group Record is a block of fields containing information pertaining to the sender's membership in a single multicast group on the interface from which the Report is sent.
        /// </summary>
        public ReadOnlyCollection<IgmpGroupRecordDatagram> GroupRecords
        {
            get
            {
                if (_groupRecords == null)
                {
                    IgmpGroupRecordDatagram[] groupRecords = new IgmpGroupRecordDatagram[NumberOfGroupRecords];
                    int offset = StartOffset + Offset.GroupRecords;
                    for (int i = 0; i != groupRecords.Length; ++i)
                    {
                        groupRecords[i] = new IgmpGroupRecordDatagram(Buffer, offset);
                        offset += groupRecords[i].Length;
                    }

                    _groupRecords = new ReadOnlyCollection<IgmpGroupRecordDatagram>(groupRecords);
                }

                return _groupRecords;
            }
        }

        public override ILayer ExtractLayer()
        {
            switch (MessageType)
            {
                case IgmpMessageType.MembershipQuery:
                    switch (QueryVersion)
                    {
                        case IgmpQueryVersion.Version1:
                            return new IgmpQueryVersion1Layer
                            {
                                GroupAddress = GroupAddress
                            };

                        case IgmpQueryVersion.Version2:
                            return new IgmpQueryVersion2Layer
                            {
                                MaxResponseTime = MaxResponseTime,
                                GroupAddress = GroupAddress
                            };

                        case IgmpQueryVersion.Version3:
                            return new IgmpQueryVersion3Layer
                            {
                                MaxResponseTime = MaxResponseTime,
                                GroupAddress = GroupAddress,
                                IsSuppressRouterSideProcessing = IsSuppressRouterSideProcessing,
                                QueryRobustnessVariable = QueryRobustnessVariable,
                                QueryInterval = QueryInterval,
                                SourceAddresses = SourceAddresses
                            };

                        default:
                            throw new InvalidOperationException("Invalid Query Version " + QueryVersion);
                    }

                case IgmpMessageType.MembershipReportVersion1:
                    return new IgmpReportVersion1Layer
                    {
                        GroupAddress = GroupAddress
                    };

                case IgmpMessageType.MembershipReportVersion2:
                    return new IgmpReportVersion2Layer
                    {
                        MaxResponseTime = MaxResponseTime,
                        GroupAddress = GroupAddress
                    };

                case IgmpMessageType.LeaveGroupVersion2:
                    return new IgmpLeaveGroupVersion2Layer
                    {
                        MaxResponseTime = MaxResponseTime,
                        GroupAddress = GroupAddress
                    };

                case IgmpMessageType.MembershipReportVersion3:
                    return new IgmpReportVersion3Layer
                    {
                        GroupRecords = GroupRecords.Select(record => record.ToGroupRecord()).ToList()
                    };

                default:
                    throw new InvalidOperationException("Invalid message type " + MessageType);
            }

        }

        internal IgmpDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static int GetQueryVersion3Length(int numSourceAddresses)
        {
            return QueryVersion3HeaderLength + IpV4Address.SizeOf * numSourceAddresses;
        }

        internal static int GetReportVersion3Length(IEnumerable<IgmpGroupRecord> igmpGroupRecords)
        {
            return HeaderLength +
                   igmpGroupRecords.Sum(record => IgmpGroupRecordDatagram.GetLength(record.SourceAddresses.Count, record.AuxiliaryData.Length));
        }

        internal static void WriteHeader(byte[] buffer, int offset,
                                         IgmpMessageType igmpMessageType, TimeSpan maxResponseTime, IpV4Address groupAddress)
        {
            buffer.Write(offset + Offset.MessageType, (byte)igmpMessageType);

            double numTenthOfASecond = (maxResponseTime.TotalSeconds * 10);
            if (numTenthOfASecond >= 256 || numTenthOfASecond < 0)
                throw new ArgumentOutOfRangeException("maxResponseTime", maxResponseTime, "must be in the range [" + TimeSpan.Zero + ", " + TimeSpan.FromSeconds(255 * 0.1) + "]");
            buffer.Write(offset + Offset.MaxResponseCode, (byte)numTenthOfASecond);

            buffer.Write(offset + Offset.GroupAddress, groupAddress, Endianity.Big);

            WriteChecksum(buffer, offset, HeaderLength);
        }

        internal static void WriteQueryVersion3(byte[] buffer, int offset,
                                                TimeSpan maxResponseTime, IpV4Address groupAddress,
                                                bool isSuppressRouterSideProcessing, byte queryRobustnessVariable, TimeSpan queryInterval,
                                                IEnumerable<IpV4Address> sourceAddresses)
        {
            // MessageType
            buffer.Write(offset + Offset.MessageType, (byte)IgmpMessageType.MembershipQuery);

            // MaxResponseCode
            if (maxResponseTime < TimeSpan.Zero || maxResponseTime > MaxVersion3MaxResponseTime)
                throw new ArgumentOutOfRangeException("maxResponseTime", maxResponseTime, "must be in the range [" + TimeSpan.Zero + ", " + MaxVersion3MaxResponseTime + "]");
            double maxResponseTimeTenthOfASecond = maxResponseTime.TotalSeconds * 10;
            byte maxResponseCode = (byte)(maxResponseTimeTenthOfASecond < 128 ? maxResponseTimeTenthOfASecond : ValueToCode((int)maxResponseTimeTenthOfASecond));
            buffer.Write(offset + Offset.MaxResponseCode, maxResponseCode);

            // GroupAddress
            buffer.Write(offset + Offset.GroupAddress, groupAddress, Endianity.Big);

            // IsSuppressRouterSideProcessing and QueryRobustnessVariable
            if (queryRobustnessVariable > MaxQueryRobustnessVariable)
                throw new ArgumentOutOfRangeException("queryRobustnessVariable", queryRobustnessVariable, "must be in range [0, 7]");
            buffer.Write(offset + Offset.QueryRobustnessVariable, (byte)(queryRobustnessVariable | (isSuppressRouterSideProcessing ? 0x08 : 0x00)));

            // QueryIntervalCode
            if (queryInterval < TimeSpan.Zero || queryInterval > MaxQueryInterval)
                throw new ArgumentOutOfRangeException("queryInterval", maxResponseTime, "must be in the range [" + TimeSpan.Zero + ", " + MaxQueryInterval + "]");
            double queryIntervalTenthOfASecond = queryInterval.TotalSeconds;
            byte queryIntervalCode = (byte)(queryIntervalTenthOfASecond < 128 ? queryIntervalTenthOfASecond : ValueToCode((int)queryIntervalTenthOfASecond));
            buffer.Write(offset + Offset.QueryIntervalCode, queryIntervalCode);

            // SourceAddresses
            int numSourceAddresses = 0;
            foreach (IpV4Address sourceAddress in sourceAddresses)
            {
                buffer.Write(offset + Offset.SourceAddresses + IpV4Address.SizeOf * numSourceAddresses, sourceAddress, Endianity.Big);
                ++numSourceAddresses;
            }

            // NumberOfSources
            buffer.Write(offset + Offset.NumberOfSources, (ushort)numSourceAddresses, Endianity.Big);

            // Checksum
            WriteChecksum(buffer, offset, QueryVersion3HeaderLength + IpV4Address.SizeOf * numSourceAddresses);
        }

        internal static void WriteReportVersion3(byte[] buffer, int offset,
                                                 IEnumerable<IgmpGroupRecord> groupRecords)
        {
            // MessageType
            buffer.Write(offset + Offset.MessageType, (byte)IgmpMessageType.MembershipReportVersion3);

            ushort numGroupRecords = 0;
            int recordOffset = offset + Offset.GroupRecords;
            foreach (IgmpGroupRecord record in groupRecords)
            {
                IgmpGroupRecordDatagram.Write(buffer, ref recordOffset, record.RecordType, record.AuxiliaryData, record.MulticastAddress, record.SourceAddresses);
                ++numGroupRecords;
            }

            // NumberOfGroupRecords
            buffer.Write(offset + Offset.NumberOfGroupRecords, numGroupRecords, Endianity.Big);

            // Checksum
            WriteChecksum(buffer, offset, recordOffset - offset);
        }

        /// <summary>
        /// IGMP is valid if the checksum is correct, the length fits the message type and data and the MaxResponseCode is 0 in messages where it is not used.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (Length < HeaderLength || !IsChecksumCorrect)
                return false;

            switch (MessageType)
            {
                case IgmpMessageType.MembershipQuery:
                    switch (QueryVersion)
                    {
                        case IgmpQueryVersion.Version1: // Version 1 actually means that the MaxResponseCode is 0.
                        case IgmpQueryVersion.Version2:
                            return Length == HeaderLength;

                        case IgmpQueryVersion.Version3:
                            return Length == GetQueryVersion3Length(NumberOfSources);

                        default:
                            return false;
                    }

                case IgmpMessageType.MembershipReportVersion1:
                    return Length == HeaderLength && MaxResponseCode == 0;

                case IgmpMessageType.LeaveGroupVersion2:
                case IgmpMessageType.MembershipReportVersion2:
                    return Length == HeaderLength;

                case IgmpMessageType.MembershipReportVersion3:
                    return MaxResponseCode == 0 &&
                           Length == HeaderLength + GroupRecords.Sum(record => record.Length) &&
                           GroupRecords.All(record => record.IsValid);

                default:
                    return false;
            }
        }

        private ushort CalculateChecksum()
        {
            uint sum = Sum16Bits(Buffer, StartOffset, Math.Min(Offset.Checksum, Length)) +
                       Sum16Bits(Buffer, StartOffset + Offset.Checksum + sizeof(ushort), Length - Offset.Checksum - sizeof(ushort));

            return Sum16BitsToChecksum(sum);
        }

        private static void WriteChecksum(byte[] buffer, int offset, int length)
        {
            buffer.Write(offset + Offset.Checksum, Sum16BitsToChecksum(Sum16Bits(buffer, offset, length)), Endianity.Big);
        }

        /// <summary>
        /// Calculates the value from the given code as follows:
        /// <pre>
        ///  0 1 2 3 4 5 6 7
        /// +-+-+-+-+-+-+-+-+
        /// |1| exp | mant  |
        /// +-+-+-+-+-+-+-+-+
        /// </pre>
        /// Value = (mant | 0x10) &lt;&lt; (exp + 3).
        /// </summary>
        private static int CodeToValue(byte code)
        {
            int mant = code & 0x0F;
            int exp = (code & 0x70) >> 4;
            return (mant | 0x10) << (exp + 3);
        }

        private static byte ValueToCode(int value)
        {
            int exp = (int)(Math.Log(value, 2) - 7);
            if (exp > 7 || exp < 0)
                throw new ArgumentOutOfRangeException("value", value, "exp " + exp + " is out of range");

            int mant = (int)(value * Math.Pow(2, -exp - 3) - 16);
            if (mant > 15 || mant < 0)
                throw new ArgumentOutOfRangeException("value", value, "mant " + mant + " is out of range");

            return (byte)(0x80 | (exp << 4) | mant);
        }

        private static readonly TimeSpan _maxMaxResponseTime = TimeSpan.FromSeconds(0.1 * 255) + TimeSpan.FromSeconds(0.1) - TimeSpan.FromTicks(1);
        private static readonly TimeSpan _maxVersion3MaxResponseTime = TimeSpan.FromSeconds(0.1 * CodeToValue(byte.MaxValue)) + TimeSpan.FromSeconds(0.1) - TimeSpan.FromTicks(1);
        private static readonly TimeSpan _maxQueryInterval = TimeSpan.FromSeconds(CodeToValue(byte.MaxValue)) + TimeSpan.FromSeconds(1) - TimeSpan.FromTicks(1);

        private bool? _isChecksumCorrect;
        private ReadOnlyCollection<IpV4Address> _sourceAddresses;
        private ReadOnlyCollection<IgmpGroupRecordDatagram> _groupRecords;
    }
}