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
    /// </pre>
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

        /// <summary>
        /// The number of bytes the group record header takes (without the source addresses and auxiliary data).
        /// </summary>
        public const int HeaderLength = 8;

        /// <summary>
        /// The type of group record included in the report message.
        /// </summary>
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
                if (_sourceAddresses == null)
                {
                    IpV4Address[] sourceAddresses = new IpV4Address[NumberOfSources];
                    for (int i = 0; i != sourceAddresses.Length; ++i)
                        sourceAddresses[i] = ReadIpV4Address(Offset.SourceAddresses + 4 * i, Endianity.Big);
                    _sourceAddresses = new ReadOnlyCollection<IpV4Address>(sourceAddresses);
                }

                return _sourceAddresses;
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
            get { return new Datagram(Buffer, StartOffset + Length - AuxiliaryDataLength, AuxiliaryDataLength); }
        }

        /// <summary>
        /// Creates an IGMP group record from the given datagram.
        /// Useful to create a new IGMP packet with group records.
        /// </summary>
        public IgmpGroupRecord ToGroupRecord()
        {
            return new IgmpGroupRecord(RecordType, MulticastAddress, SourceAddresses, AuxiliaryData);
        }

        internal IgmpGroupRecordDatagram(byte[] buffer, int offset)
            : base(buffer, offset,
                   buffer.Length - offset < HeaderLength
                       ? buffer.Length - offset
                       : Math.Min(buffer.Length - offset, HeaderLength +
                                                          4 * buffer.ReadUShort(offset + Offset.NumberOfSources, Endianity.Big) +
                                                          4 * buffer.ReadByte(offset + Offset.AuxiliaryDataLength)))
        {
        }

        internal static int GetLength(int numSourceAddresses, int auxiliaryDataLength)
        {
            return HeaderLength + IpV4Address.SizeOf * numSourceAddresses + auxiliaryDataLength;
        }

        internal static void Write(byte[] buffer, ref int offset, IgmpRecordType recordType, Datagram auxiliaryData, IpV4Address multicastAddress, ReadOnlyCollection<IpV4Address> sourceAddresses)
        {
            buffer.Write(offset + Offset.RecordType, (byte)recordType);
            buffer.Write(offset + Offset.AuxiliaryDataLength, (byte)(auxiliaryData.Length / 4));
            int numSourceAddresses = sourceAddresses.Count;
            buffer.Write(offset + Offset.NumberOfSources, (ushort)numSourceAddresses, Endianity.Big);
            buffer.Write(offset + Offset.MulticastAddress, multicastAddress, Endianity.Big);
            for (int i = 0; i != numSourceAddresses; ++i)
                buffer.Write(offset + Offset.SourceAddresses + IpV4Address.SizeOf * i, sourceAddresses[i], Endianity.Big);

            offset += HeaderLength + numSourceAddresses * IpV4Address.SizeOf;
            buffer.Write(ref offset, auxiliaryData);
        }

        /// <summary>
        /// The record is valid if the length is correct according to the header fields.
        /// </summary>
        protected override bool  CalculateIsValid()
        {
            return Length >= HeaderLength &&
                   Length == HeaderLength + IpV4Address.SizeOf * NumberOfSources + AuxiliaryDataLength;
        }

        private ReadOnlyCollection<IpV4Address> _sourceAddresses;
    }
}