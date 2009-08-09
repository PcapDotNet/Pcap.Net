namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// Contains the common part of UDP and TCP.
    /// 
    /// Format:
    /// +-----+-------------+----------+-----+-----+-----+-----+-----+-----+------------------+
    /// | Bit | 0-4         | 4-9      | 10  | 11  | 12  | 13  | 14  | 15  | 16-31            |
    /// +-----+-------------+----------+-----+-----+-----+-----+-----+-----+------------------+
    /// | 0   | Source Port                                                | Destination Port |
    /// +-----+------------------------------------------------------------+------------------+
    /// | 32  | Sequence Number                                                               |
    /// +-----+-------------------------------------------------------------------------------+
    /// </summary>
    public abstract class TransportDatagram : Datagram
    {
        private static class Offset
        {
            public const int SourcePort = 0;
            public const int DestinationPort = 2;
        }

        /// <summary>
        /// Indicates the port of the sending process.
        /// In UDP, this field is optional and may only be assumed to be the port 
        /// to which a reply should be addressed in the absence of any other information.
        /// If not used in UDP, a value of zero is inserted.
        /// </summary>
        public ushort SourcePort
        {
            get { return ReadUShort(Offset.SourcePort, Endianity.Big); }
        }

        /// <summary>
        /// Destination Port has a meaning within the context of a particular internet destination address.
        /// </summary>
        public ushort DestinationPort
        {
            get { return ReadUShort(Offset.DestinationPort, Endianity.Big); }
        }

        protected static void WriteHeader(byte[] buffer, int offset, ushort sourcePort, ushort destinationPort)
        {
            buffer.Write(offset + Offset.SourcePort, sourcePort, Endianity.Big);
            buffer.Write(offset + Offset.DestinationPort, destinationPort, Endianity.Big);
        }

        protected TransportDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}