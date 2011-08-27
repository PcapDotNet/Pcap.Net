using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// A raw packet.
    /// Includes all packet layers as taken from an adapter including the type of the datalink.
    /// Immutable.
    /// </summary>
    public sealed class Packet : IList<byte>, IEquatable<Packet>
    {
        /// <summary>
        /// Creates a packet from a string that represents bytes in a hexadecimal format.
        /// </summary>
        public static Packet FromHexadecimalString(string value, DateTime timestamp, DataLinkKind dataLink)
        {
            if (value == null) 
                throw new ArgumentNullException("value");

            byte[] bytes = new byte[value.Length / 2];

            for (int i = 0; i < value.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            }

            return new Packet(bytes, timestamp, dataLink);
        }

        /// <summary>
        /// Create a packet from an array of bytes.
        /// </summary>
        /// <param name="data">The bytes of the packet. This array should not be changed after creating the packet until the packet is no longer used.</param>
        /// <param name="timestamp">A timestamp of the packet - when it was captured.</param>
        /// <param name="dataLink">The type of the datalink of the packet.</param>
        public Packet(byte[] data, DateTime timestamp, DataLinkKind dataLink)
            :this(data, timestamp, new DataLink(dataLink))
        {
        }

        /// <summary>
        /// Create a packet from an array of bytes.
        /// </summary>
        /// <param name="data">The bytes of the packet. This array should not be changed after creating the packet until the packet is no longer used.</param>
        /// <param name="timestamp">A timestamp of the packet - when it was captured.</param>
        /// <param name="dataLink">The type of the datalink of the packet.</param>
        public Packet(byte[] data, DateTime timestamp, IDataLink dataLink)
        {
            _data = data;
            _timestamp = timestamp;
            _dataLink = dataLink;
        }

        /// <summary>
        /// The number of bytes this packet take.
        /// </summary>
        public int Length
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// The time this packet was captured.
        /// </summary>
        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        /// <summary>
        /// The type of the datalink of the device this packet was captured from.
        /// </summary>
        public IDataLink DataLink
        {
            get { return _dataLink; }
        }

        /// <summary>
        /// The underlying array of bytes.
        /// When taking this array the caller is responsible to make sure this array will not be modified while the packet is still in use.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Buffer
        {
            get { return _data; }
        }

        /// <summary>
        /// Equals means that the packets have equal data.
        /// </summary>
        /// <param name="other">The packet to compare this packet to.</param>
        /// <returns>True iff the packets have equal data.</returns>
        public bool Equals(Packet other)
        {
            return (other != null && Length == other.Length && this.SequenceEqual(other));
        }

        /// <summary>
        /// Equals means that the packets have equal data.
        /// </summary>
        /// <param name="obj">The packet to compare this packet to.</param>
        /// <returns>True iff the packets have equal data.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Packet);
        }

        /// <summary>
        /// The hash code of a packet is the xor of all its bytes. Each byte is xored with the next 8 bits of the integer.
        /// </summary>
        public override int GetHashCode()
        {
            return this.BytesSequenceGetHashCode();
        }

        /// <summary>
        /// The Packet string contains the datalink and the length.
        /// </summary>
        public override string ToString()
        {
            return typeof(Packet).Name + " <" + DataLink + ", " + Length + ">";
        }

        /// <summary>
        /// Returns an enumerator that iterates through the bytes of the packet.
        /// </summary>
        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)Buffer).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the first offset in the packet that contains the given byte.
        /// </summary>
        public int IndexOf(byte item)
        {
            return ((IList<byte>)Buffer).IndexOf(item);
        }

        /// <summary>
        /// This operation is invalid because the object is immutable.
        /// </summary>
        public void Insert(int index, byte item)
        {
            throw new InvalidOperationException("Immutable collection");
        }

        /// <summary>
        /// This operation is invalid because the object is immutable.
        /// </summary>
        public void RemoveAt(int index)
        {
            throw new InvalidOperationException("Immutable collection");
        }

        /// <summary>
        /// Returns the value of the byte in the given offset.
        /// Set operation is invalid because the object is immutable.
        /// </summary>
        public byte this[int index]
        {
            get { return Buffer[index]; }
            set { throw new InvalidOperationException("Immutable collection"); ; }
        }

        /// <summary>
        /// This operation is invalid because the object is immutable.
        /// </summary>
        public void Add(byte item)
        {
            throw new InvalidOperationException("Immutable collection");
        }

        /// <summary>
        /// This operation is invalid because the object is immutable.
        /// </summary>
        public void Clear()
        {
            throw new InvalidOperationException("Immutable collection");
        }

        /// <summary>
        /// Determines whether the packet contains a specific byte.
        /// </summary>
        public bool Contains(byte item)
        {
            return Buffer.Contains(item);
        }

        /// <summary>
        /// Copies the bytes of the packet to a buffer, starting at a particular offset.
        /// </summary>
        public void CopyTo(byte[] array, int arrayIndex)
        {
            Buffer.BlockCopy(0, array, arrayIndex, Length);
        }

        /// <summary>
        /// This operation is invalid because the object is immutable.
        /// </summary>
        public bool Remove(byte item)
        {
            throw new InvalidOperationException("Immutable collection");
        }

        /// <summary>
        /// Returns the number of bytes in this packet.
        /// </summary>
        public int Count
        {
            get { return Length; }
        }

        /// <summary>
        /// True since this object is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        ///<summary>
        /// True iff the packet is valid.
        /// The packet is valid unless an invalid part of it was found.
        /// Examples for invalid parts:
        /// * Bad checksum.
        /// * An illegal value for a field.
        /// * Length of packet is too short for a header or according to a length field.
        ///</summary>
        public bool IsValid
        {
            get
            {
                if (_isValid == null)
                    _isValid = CalculateIsValid();
                return _isValid.Value;
            }
        }

        /// <summary>
        /// Takes the entire packet as an Ethernet datagram.
        /// </summary>
        public EthernetDatagram Ethernet
        {
            get { return _ethernet ?? (_ethernet = new EthernetDatagram(Buffer, 0, Length)); }
        }

        private bool CalculateIsValid()
        {
            switch (DataLink.Kind)
            {
                case DataLinkKind.Ethernet:
                    return Ethernet.IsValid;
                default:
                    return false;
            }
        }

        private readonly byte[] _data;
        private readonly DateTime _timestamp;
        private readonly IDataLink _dataLink;
        private bool? _isValid;

        private EthernetDatagram _ethernet;
    }
}
