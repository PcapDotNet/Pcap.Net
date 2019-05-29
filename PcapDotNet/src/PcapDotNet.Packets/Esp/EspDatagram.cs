namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// RFC 4303
    /// 
    /// The Encapsulating Security Payload (ESP) header is designed to
    /// provide a mix of security services in IPv4 and IPv6[DH98].  ESP may
    /// be applied alone, in combination with AH[Ken - AH], or in a nested
    /// fashion(see the Security Architecture document[Ken - Arch]).
    /// Security services can be provided between a pair of communicating
    /// hosts, between a pair of communicating security gateways, or between
    /// a security gateway and a host.For more details on how to use ESP
    /// and AH in various network environments, see the Security Architecture
    /// document[Ken - Arch].
    /// 
    /// The ESP header is inserted after the IP header and before the next
    /// layer protocol header (transport mode) or before an encapsulated IP
    /// header (tunnel mode).
    /// 
    /// Format:
    /// <pre>
    ///  0                   1                   2                   3
    ///  0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ ----
    /// |               Security Parameters Index(SPI)                  | ^Int.
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ |Cov-
    /// |                      Sequence Number                          | |ered
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ | ----
    /// |                    Payload Data* (variable)                   | |   ^
    /// ~                                                               ~ |   |
    /// |                                                               | |Conf.
    /// +               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ |Cov-
    /// |               |     Padding (0-255 bytes)                     | |ered*
    /// +-+-+-+-+-+-+-+-+               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ |   |
    /// |                               |  Pad Length   | Next Header   | v   v
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+ ------
    /// |         Integrity Check Value-ICV   (variable)                |
    /// ~                                                               ~
    /// |                                                               |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </pre>
    /// 
    /// NOTE: The payload includes everything after the sequence number.
    /// </summary>
    public abstract class EspDatagram : Datagram
    {
        /// <summary>
        /// The number of bytes the datagram header takes.
        /// </summary>
        public const int HeaderLength = 8;

        internal static class Offset
        {
            public const int SecurityParametersIndex = 0;
            public const int SequenceNumber = 4;
        }

        /// <summary>
        /// The Security Parameters Index (SPI).
        /// </summary>
        public uint SecurityParametersIndex
        {
            get { return ReadUInt(Offset.SecurityParametersIndex, Endianity.Big); }
        }

        /// <summary>
        /// Destination Port has a meaning within the context of a particular internet destination address.
        /// </summary>
        public uint SequenceNumber
        {
            get { return ReadUInt(Offset.SequenceNumber, Endianity.Big); }
        }

        /// <summary>
        /// The payload of the ESP datagram.
        /// </summary>
        public Datagram Payload
        {
            get { return new Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength); }
        }

        internal EspDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}
