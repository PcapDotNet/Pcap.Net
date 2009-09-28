using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Igmp
{
    public class IgmpGroupRecord : IEquatable<IgmpGroupRecord>
    {
        public IgmpGroupRecord(IgmpRecordType recordType, IpV4Address multicastAddress, ReadOnlyCollection<IpV4Address> sourceAddresses, Datagram auxiliaryData)
        {
            if (auxiliaryData.Length % 4 != 0)
                throw new ArgumentException("Auxiliary data length must divide by 4 and can't be " + auxiliaryData.Length, "auxiliaryData");

            RecordType = recordType;
            MulticastAddress = multicastAddress;
            SourceAddresses = new ReadOnlyCollection<IpV4Address>(sourceAddresses);
            AuxiliaryData = auxiliaryData;
        }

        public IgmpGroupRecord(IgmpRecordType recordType, IpV4Address multicastAddress, IList<IpV4Address> sourceAddresses, Datagram auxiliaryData)
            : this(recordType, multicastAddress, new ReadOnlyCollection<IpV4Address>(sourceAddresses), auxiliaryData)
        {
        }

        /// <summary>
        /// The type of group record included in the report message.
        /// </summary>
        public IgmpRecordType RecordType
        {
            get; private set;
        }

        /// <summary>
        /// The Multicast Address field contains the IP multicast address to which this Group Record pertains.
        /// </summary>
        public IpV4Address MulticastAddress
        {
            get; private set;
        }

        /// <summary>
        /// The Source Address [i] fields are a vector of n IP unicast addresses, 
        /// where n is the value in this record's Number of Sources (N) field.
        /// </summary>
        public ReadOnlyCollection<IpV4Address> SourceAddresses
        {
            get; private set;
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
            get; private set;
        }

        public override string ToString()
        {
            return RecordType + " " + MulticastAddress + " " + SourceAddresses.SequenceToString(", ", "[", "]") + " Aux=" + AuxiliaryData.Length;
        }

        public bool Equals(IgmpGroupRecord other)
        {
            if (other == null)
                return false;

            return RecordType == other.RecordType &&
                   MulticastAddress == other.MulticastAddress &&
                   SourceAddresses.SequenceEqual(other.SourceAddresses) &&
                   AuxiliaryData.Equals(other.AuxiliaryData);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IgmpGroupRecord);
        }
    }
}