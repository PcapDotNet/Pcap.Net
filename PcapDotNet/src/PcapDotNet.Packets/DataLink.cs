using System;

namespace PcapDotNet.Packets
{

    /// <summary>
    /// Represents the DataLink type.
    /// </summary>
    public struct DataLink : IDataLink, IEquatable<DataLink>
    {
        /// <summary>
        /// Etherent DataLink.
        /// </summary>
        public static DataLink Ethernet
        {
            get { return _ethernet; }
        }

        /// <summary>
        /// IPv4 DataLink.
        /// </summary>
        public static DataLink IpV4
        {
            get { return _ipV4; }
        }

        /// <summary>
        /// Create the DataLink from a kind.
        /// </summary>
        public DataLink(DataLinkKind kind)
        {
            _kind = kind;
        }

        /// <summary>
        /// The kind of the DataLink.
        /// </summary>
        public DataLinkKind Kind
        {
            get { return _kind; }
        }

        /// <summary>
        /// Two DataLinks are equal if they are of the same kind.
        /// </summary>
        public bool Equals(DataLink other)
        {
            return Kind == other.Kind;
        }

        /// <summary>
        /// Two DataLinks are equal if they are of the same type and the same kind.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is DataLink &&
                    Equals((DataLink)obj));
        }

        /// <summary>
        /// Two DataLinks are equal if they are of the same kind.
        /// </summary>
        public static bool operator ==(DataLink dataLink1, DataLink dataLink2)
        {
            return dataLink1.Equals(dataLink2);
        }

        /// <summary>
        /// Two DataLinks are different if they have different kinds.
        /// </summary>
        public static bool operator !=(DataLink dataLink1, DataLink dataLink2)
        {
            return !(dataLink1 == dataLink2);
        }

        /// <summary>
        /// The hash code of the datalink is the hash code of its kind.
        /// </summary>
        public override int GetHashCode()
        {
            return _kind.GetHashCode();
        }

        /// <summary>
        /// The string is the kind's string.
        /// </summary>
        public override string ToString()
        {
            return Kind.ToString();
        }

        private static readonly DataLink _ethernet = new DataLink(DataLinkKind.Ethernet);
        private static readonly DataLink _ipV4 = new DataLink(DataLinkKind.IpV4);
        private readonly DataLinkKind _kind;
    }
}