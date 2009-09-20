using System;
using System.Collections.ObjectModel;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// Each Group Record is a block of fields containing information pertaining 
    /// to the sender's membership in a single multicast group on the interface from which the Report is sent.
    /// A Group Record has the following internal format:
    /// <pre>
    /// +-----+-------------+--------------+--------+--------------+
    /// | Bit | 0-7         | 8-15         | 16-31  |              |
    /// +-----+-------------+--------------+--------+--------------+
    /// | 0   | Record Type | Aux Data Len | Number of Sources (N) |
    /// +-----+-------------+--------------+--------+--------------+
    /// | 32  | Multicast Address                                  |
    /// +-----+----------------------------------------------------+
    /// | 64  | Source Address [1]                                 |
    /// +-----+----------------------------------------------------+
    /// | 96  | Source Address [2]                                 |
    /// +-----+----------------------------------------------------+
    /// .     .                         .                          .
    /// .     .                         .                          .
    /// +-----+----------------------------------------------------+
    /// | 32  | Source Address [N]                                 |
    /// | +   |                                                    |
    /// | 32N |                                                    |
    /// +-----+----------------------------------------------------+
    /// | 64  | Auxiliary Data                                     |
    /// . +   .                                                    .
    /// . 32N .                                                    .
    /// .     .                                                    .
    /// |     |                                                    |
    /// +-----+----------------------------------------------------+
    /// </summary>
    public class IgmpGroupRecordDatagram : Datagram
    {
        private static class Offset
        {
            public const int RecordType = 0;
            public const int AuxiliaryDataLength = 1;
            public const int NumberOfSources = 2;
            public const int MulticastAddress = 4;
            public const int SourceAddresses = 8;
        }

        public const int HeaderMinimumLength = 8;

        public IgmpRecordType RecordType
        {
            get { return (IgmpRecordType)this[Offset.RecordType]; }
        }

        /// <summary>
        /// The Aux Data Len field contains the length of the Auxiliary Data field in this Group Record, in bytes (after a translation from 32 bit words length).  
        /// It may contain zero, to indicate the absence of any auxiliary data.
        /// </summary>
        public int AuxiliaryDataLength
        {
            get { return 4 * this[Offset.AuxiliaryDataLength]; }
        }

        /// <summary>
        /// The Number of Sources (N) field specifies how many source addresses are present in this Group Record.
        /// </summary>
        public int NumberOfSources
        {
            get { return ReadUShort(Offset.NumberOfSources, Endianity.Big); }
        }

        /// <summary>
        /// The Multicast Address field contains the IP multicast address to which this Group Record pertains.
        /// </summary>
        public IpV4Address MulticastAddress
        {
            get { return ReadIpV4Address(Offset.MulticastAddress, Endianity.Big); }
        }

        /// <summary>
        /// The Source Address [i] fields are a vector of n IP unicast addresses, 
        /// where n is the value in this record's Number of Sources (N) field.
        /// </summary>
        public ReadOnlyCollection<IpV4Address> SourceAddresses
        {
            get
            {
                IpV4Address[] sourceAddresses = new IpV4Address[NumberOfSources];
                for (int i = 0; i != sourceAddresses.Length; ++i)
                    sourceAddresses[i] = ReadIpV4Address(Offset.SourceAddresses + 4 * i, Endianity.Big);
                return new ReadOnlyCollection<IpV4Address>(sourceAddresses);
            }
        }

        /// <summary>
        /// The Auxiliary Data field, if present, contains additional information pertaining to this Group Record.  
        /// The protocol specified in this document, IGMPv3, does not define any auxiliary data.  
        /// Therefore, implementations of IGMPv3 MUST NOT include any auxiliary data (i.e., MUST set the Aux Data Len field to zero) in any transmitted Group Record, 
        /// and MUST ignore any auxiliary data present in any received Group Record.  
        /// The semantics and internal encoding of the Auxiliary Data field are to be defined by any future version or extension of IGMP that uses this field.
        /// </summary>
        public Datagram AuxiliaryData
        {
            get { return new Datagram(Buffer, StartOffset + Offset.AuxiliaryDataLength, AuxiliaryDataLength); }
        }

        internal IgmpGroupRecordDatagram(byte[] buffer, int offset)
            : base(buffer, offset,
                   buffer.Length - offset < HeaderMinimumLength
                       ? buffer.Length - offset
                       : Math.Min(buffer.Length - offset, HeaderMinimumLength +
                                                          4 * buffer.ReadUShort(offset + Offset.NumberOfSources, Endianity.Big) +
                                                          4 * buffer.ReadByte(offset + Offset.AuxiliaryDataLength)))
        {
        }

        protected override bool CalculateIsValid()
        {
            return Length >= HeaderMinimumLength &&
                   Length == HeaderMinimumLength + 4 * NumberOfSources + AuxiliaryDataLength;
        }
    }
}