using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// A raw packet.
    /// Includes all packet layers as taken from an adapter including the type of the datalink.
    /// Immutable.
    /// </summary>
    public class Packet : IList<byte>, IEquatable<Packet>
    {
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
            return (Length == other.Length && this.SequenceEqual(other));
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
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hashCode = 0;
            int offset = 0;
            for (; offset < _data.Length - 3; offset += 4)
                hashCode ^= _data.ReadInt(offset, Endianity.Small);
            if (offset < _data.Length - 1)
                hashCode ^= _data.ReadShort(offset, Endianity.Small);
            if (offset < _data.Length)
                hashCode ^= _data[offset] >> 2;
            return hashCode;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return typeof(Packet).Name + " <" + DataLink + ", " + Length + ">";
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)Buffer).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
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
        /// Gets the element at the specified index.
        /// Set operation is invalid because the object is immutable.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set.</exception>
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
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(byte item)
        {
            return Buffer.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
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
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
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
        /// Takes the entire packet as an ethernet datagram.
        /// </summary>
        public EthernetDatagram Ethernet
        {
            get
            {
                if (_ethernet == null)
                    _ethernet = new EthernetDatagram(Buffer, 0, Length);
                return _ethernet;
            }
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
